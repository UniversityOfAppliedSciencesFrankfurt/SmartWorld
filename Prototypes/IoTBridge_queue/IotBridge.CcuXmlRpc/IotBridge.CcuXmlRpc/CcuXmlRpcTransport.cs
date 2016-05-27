using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using XmlRpcLib;

namespace IotBridge.CcuXmlRpc
{
    /// <summary>
    /// This class implements an Iot transport to devices which are reachable by CCU XML RPC. 
    /// </summary>
    public class CcuXmlRpcBridge : IBridgeTransport
    {

        public void OnMessage(Action<Message> onReceiveMsg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Message Receive(Dictionary<string, object> args = null)
        {
            string text;
            CCU ccu = new CCU();
            ccu.Initialize(args["ccuAddress"].ToString(), Convert.ToInt16(args["timeOut"]));
            string sensor = args["sensor"].ToString();
            string action = args["action"].ToString();

            try
            {
                if (String.Compare(sensor, "DEVICE") == 0)
                {
                    text = ccu.DeviceList();//This API lists the device connected to the CCU
                }
                else
                {
                    text = ccu.GetValue(sensor, action);//This API gets the value of the particular sensor
                }
            }
            catch
            {
                throw;
            }


            Message msg = new Message(text);
            return msg;
            throw new NotImplementedException();
        }

        public void Send(Message msg, Dictionary<string, object> args = null)
        {

            CCU ccu = new CCU();
            ccu.Initialize(args["ccuAddress"].ToString(), Convert.ToInt16(args["timeOut"]));
            string sensor = args["sensor"].ToString();
            string action = args["action"].ToString();
            string value = args["value"].ToString();

            try
            {
                ccu.SetValue(sensor, action, value);

            }
            catch
            {
                throw;
            }

        }


        public void SendReceiveAckonwledgeResult(string msgId, Exception error, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public string Name
        {
            get { return "CcuXmlRpcBridge"; }
        }
    }
}
