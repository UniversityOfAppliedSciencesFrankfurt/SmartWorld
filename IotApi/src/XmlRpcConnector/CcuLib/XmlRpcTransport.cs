//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using IoTBridge;
//using XmlRpcCore;

//namespace CcuLib
//{
//    public class XMLRPCTransport : IIotApi
//    {
//        Ccu m_Ccu = new Ccu();

//        public string Name
//        {
//            get { return "CCUConnector"; }
//        }
        
//        Task IIotApi.Open(Dictionary<string, object> args)
//        {
//            if (args == null || !args.ContainsKey("CcuUri"))
//                throw new ArgumentException("Arguments must contain value for 'CcuUri'!");

//            return Task.Run(() =>
//            {
//                string CcuUri = args["CcuUri"] as string;
//                if (args.ContainsKey("timeOut"))
//                {
//                    string timeOut = args["timeOut"] as string;
//                    m_Ccu.SetUriAndTimeOut(CcuUri, new TimeSpan(Convert.ToInt32(timeOut)));
//                }
//                else m_Ccu.SetUriAndTimeOut(CcuUri);

//            });
//        }

//        public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
//        {
//            throw new NotImplementedException();
//        }

//        public Task ReceiveAsync(Func<object, bool> onSuccess = null, Func<Exception, bool> onError = null, int timeout = 60000, Dictionary<string, object> args = null)
//        {
//            throw new NotImplementedException();
//        }

//        public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
//        {
//            throw new NotImplementedException();
//        }

//        public async Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
//        {
//            string message;

//            try
//            {
//                //transfer the local request (from Console or UW Apps) into Message type
//                Message receivedMessage = (Message)sensorMessage;
//                var msg = receivedMessage.GetBody<string>();
                
//                //Create a MethodCall from local request
//                m_Ccu.PrepareMethodCall(msg);

//                //Send Request
//                MethodResponse response = await m_Ccu.SendRequest();

//                //GetListDevices has different-formatted response. Therefore we have to distinguish the request whether it is the GetListDevice method or not
//                if (m_Ccu.isGetList) message = m_Ccu.GetListDevices(m_Ccu.m_CcuResponse);
//                else
//                {
//                    if (!m_Ccu.isGetMethod)
//                    {
//                        // Set Methods do not return any value. Detecting no value means the operation is done
//                        if (m_Ccu.m_CcuResponse.ReceiveParams.Count() == 0) message = "Operation is done!";

//                        // Set methods can not return any value
//                        else throw new InvalidOperationException("The operation cannot return any value!");
//                    }
//                    else
//                    {
//                        // Get methods must return a value, if not it must be an error
//                        if (m_Ccu.m_CcuResponse.ReceiveParams.Count() == 0) throw new InvalidOperationException("No value returned or error");

//                        // Collecting the returned value
//                        else message = m_Ccu.m_CcuResponse.ReceiveParams.First().Value.ToString();
//                    }
//                }

//                try
//                {
//                    onSuccess?.Invoke(new List<object> { message });
//                }
//                catch (Exception messageException)
//                {
//                    throw messageException;
//                }

//            }
//            catch (XmlRpcFaultException faultEx)
//            {
//                message = faultEx.Message;
//                onError?.Invoke(new List<object> { message }, faultEx);
//            }
//        }

//        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
