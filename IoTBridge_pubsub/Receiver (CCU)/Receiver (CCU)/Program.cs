using IotBridge;
using IotBridge.CcuXmlRpc;
using IotBridge.SbTransport;
using XmlRpcLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

namespace Receiver__CCU_
{
    class Program
    {
        public static SbTopicTransport sbPubsubTrans = new SbTopicTransport();
        public static SbQueueTransport sbQueueTrans = new SbQueueTransport();
        public static CcuXmlRpcBridge xmlrpcTrans = new CcuXmlRpcBridge();
        public static Dictionary<string, object> sbArgs_1 = new Dictionary<string, object>();
        public static Dictionary<string, object> sbArgs_2 = new Dictionary<string, object>();
        public static Dictionary<string, object> xmlrpcArgs = new Dictionary<string, object>();
        public static Message msg = new Message();

        static void Main(string[] args)
        {
            initalizeArgs();

            while (true)
            {
                ReceiveCommand(msg);
            }
        }

        //--------------------------------------------------------------------------------------------//

        public static void ReceiveCommand(Message message)
        {

            message = sbPubsubTrans.Receive(sbArgs_1);


            if (message != null)
            {
                string text = message.GetBody<object>().ToString();
                //Console.WriteLine(text);//Uncomment it for Debugging
                ProcessCommand(text);
            }
        }

        //-------------------------------------------------------------------------------------//

        public static void ProcessCommand(string text)
        {
            Message message;
            message = new Message("Error");// If this message appears in the Console then there is a failure in the operation
            string[] parameters = text.Split(' ');
            xmlrpcArgs["sensor"] = parameters[0];
            xmlrpcArgs["action"] = parameters[1];
            xmlrpcArgs["value"] = parameters[2];

            if (parameters[2] == "get")
            {
                try
                {
                    string value = xmlrpcTrans.Receive(xmlrpcArgs).GetBody<object>().ToString();
                    message = new Message(value);
                    //Console.WriteLine("Done");//Uncomment it for Debugging
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);Uncomment it for Debugging
                    message = new Message(ex.Message);
                }
            }
            else
            {
                try
                {
                    xmlrpcTrans.Send(msg, xmlrpcArgs);
                    //Console.WriteLine("Done");//Uncomment it for Debugging
                    message = new Message("Operation is Successful");
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message);Uncomment it for Debugging
                    message = new Message(ex.Message);
                }
            }

            sbQueueTrans.Send(message, sbArgs_2);

        }

        //----------------------------------------------------------------------------------------------------//

        static public void initalizeArgs()
        {
            xmlrpcArgs.Add("sensor", "");
            xmlrpcArgs.Add("action", "");
            xmlrpcArgs.Add("value", "");
            xmlrpcArgs.Add("timeOut", "5000");
            xmlrpcArgs.Add("ccuAddress", "http://192.168.0.222:2001");

            var ConnStr = ConfigurationManager.AppSettings["ConnectionString"];
            var queueName_1 = ConfigurationManager.AppSettings["QueueName_1"];
            var topicName_1 = ConfigurationManager.AppSettings["TopicName_1"];
            var subscriptionName_1 = ConfigurationManager.AppSettings["SubscriptionName_1"];
            sbArgs_2.Add("ConnStr", ConnStr);
            sbArgs_2.Add("QueueName", queueName_1);
            sbArgs_1.Add("ConnStr", ConnStr);
            sbArgs_1.Add("TopicName", topicName_1);
            sbArgs_1.Add("SubscriptionName", subscriptionName_1);
        }
    }
}
