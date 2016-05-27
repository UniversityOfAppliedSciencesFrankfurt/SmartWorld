using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotBridge.SbTransport;

namespace IotBridge.SbTransport
{
    public class SbQueueTransport : IBridgeTransport
    {
        public string Name
        {
            get { return "SbQueueTransport"; }
        }

        public void OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args = null)
        {
            string queuePath = null;
            ReceiveMode receiveMode = ReceiveMode.PeekLock;

            if (args.ContainsKey("queuePath"))
                queuePath = (string)args["queuePath"];

            if (args.ContainsKey("receiveMode"))
                receiveMode = (ReceiveMode)args["receiveMode"];

            var opts = new OnMessageOptions();
            if (args.ContainsKey("AutoComplete"))
                opts.AutoComplete = (bool)args["AutoComplete"];

            if (args.ContainsKey("MaxConcurrentCalls"))
                opts.MaxConcurrentCalls = (int)args["MaxConcurrentCalls"];

            if (args.ContainsKey("AutoRenewTimeout"))
                opts.AutoRenewTimeout = (TimeSpan)args["AutoRenewTimeout"];

            QueueClient client = QueueClient.CreateFromConnectionString("todo", queuePath, receiveMode);

            client.OnMessage((sbMsg) =>
            {
                Message msg = sbMsg.ToMessage();
                onReceiveMsg(msg);
            }, opts);
        }

        public Message Receive(Dictionary<string, object> args = null)
        {

            string m_QueueName = args["QueueName"].ToString();
            string m_ConnStr = args["ConnStr"].ToString();
            Message msg = new Message();
            QueueClient client = QueueClient.CreateFromConnectionString(m_ConnStr, m_QueueName, ReceiveMode.ReceiveAndDelete);
            var sbMsg = client.Receive(TimeSpan.FromMinutes(2));
            msg = MsgConvertor.ToMessage(sbMsg);
            return msg;

            //throw new NotImplementedException();

        }

        public void Send(Message msg, Dictionary<string, object> args = null)
        {

            string m_QueueName = args["QueueName"].ToString();
            string m_ConnStr = args["ConnStr"].ToString();

            QueueClient client = QueueClient.CreateFromConnectionString(m_ConnStr, m_QueueName);
            client.Send(MsgConvertor.FromMessage(msg));
        }


        public void SendReceiveAckonwledgeResult(string msgId, Exception error, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
