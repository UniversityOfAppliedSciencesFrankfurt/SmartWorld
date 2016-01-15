using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IotBridge.SbTransport
{
    public class SbEventHubTransport : IBridgeTransport
    {
        public string Name
        {
            get { return "SbEventHubTransport"; }
        }

        public void OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Message Receive(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void SendReceiveAckonwledgeResult(string msgId, Exception error, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void Send(Message msg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
