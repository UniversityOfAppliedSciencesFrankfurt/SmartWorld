using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IotBridge.SbTransport;

namespace IotBridge.SbTransport
{
    /// <summary>
    /// This transport is used to bridge connection to Service Bus Topics.
    /// </summary>
    public class SbTopicTransport : IBridgeTransport
    {
        public string Name
        {
            get { return "SbTopicTransport"; }
        }

        /// <summary>
        /// Invoked when the message is received on subscription.
        /// </summary>
        /// <param name="onReceiveMsg">Method which will be invoked when message arrives from Service Bus subscription</param>
        /// <param name="args"></param>

        public void OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args = null)
        {
            string topicPath = null;
            string subscriptionName = null;

            if (args.ContainsKey("topicPath"))
                topicPath = (string)args["topicPath"];

            if (args.ContainsKey("subscriptionName"))
                subscriptionName = (string)args["subscriptionName"];

            var opts = new OnMessageOptions();
            if (args.ContainsKey("AutoComplete"))
                opts.AutoComplete = (bool)args["AutoComplete"];

            if (args.ContainsKey("MaxConcurrentCalls"))
                opts.MaxConcurrentCalls = (int)args["MaxConcurrentCalls"];

            if (args.ContainsKey("AutoRenewTimeout"))
                opts.AutoRenewTimeout = (TimeSpan)args["AutoRenewTimeout"];

            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString("todo", topicPath, subscriptionName);

            client.OnMessage((sbMsg) =>
            {
                Message msg = sbMsg.ToMessage();
                onReceiveMsg(msg);
            }, opts);
        }


        /// <summary>
        /// Receieves the message from Service Bus Subscription
        /// </summary>
        /// <param name="args">topicPath, subscriptionName, ReceiveMode.</param>
        /// <returns></returns>
        public Message Receive(Dictionary<string, object> args = null)
        {
            string TopicName = args["TopicName"].ToString();
            string ConnStr = args["ConnStr"].ToString();
            string SubscriptionName = args["SubscriptionName"].ToString();

            Message msg = new Message();
            msg = null;
            SubscriptionClient client = SubscriptionClient.CreateFromConnectionString(ConnStr, TopicName, SubscriptionName, ReceiveMode.ReceiveAndDelete);
            var sbMsg = client.Receive(TimeSpan.FromSeconds(2));
            if (sbMsg != null)
            {
                msg = MsgConvertor.ToMessage(sbMsg);
            }
            return msg;
        }


        /// <summary>
        /// SEnds a message to Service Bus topic.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="args">topicPath, subscriptionName.</param>
        public void Send(Message msg, Dictionary<string, object> args = null)
        {
            string TopicName = args["TopicName"].ToString();
            string ConnStr = args["ConnStr"].ToString();

            TopicClient client = TopicClient.CreateFromConnectionString(ConnStr, TopicName);
            client.Send(MsgConvertor.FromMessage(msg));

        }


        public void SendReceiveResult(string msgId, Exception error, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnMessageSendResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        void IBridgeTransport.OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        Message IBridgeTransport.Receive(Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        void IBridgeTransport.SendReceiveAckonwledgeResult(string msgId, Exception error, Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        void IBridgeTransport.Send(Message msg, Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        void IBridgeTransport.OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }
    }
}
