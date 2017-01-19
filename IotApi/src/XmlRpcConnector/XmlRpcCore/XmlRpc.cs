using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    public class XmlRpc : ISendModule
    {
        private Uri m_Uri;
        private TimeSpan m_TimeOut;
        private bool m_Mock;

        public ISendModule NextSendModule { get; set; }

        /// <summary>
        /// Connection open
        /// </summary>
        /// <param name="args"> Necessary arguments to open connection </param>
        public void Open(Dictionary<string, object> args)
        {
            if (args == null || !args.ContainsKey("Uri"))
                throw new ArgumentException("Arguments must contain value for 'Uri'!");
            else
                m_Uri = new Uri(args["Uri"] as string);

            if (args.ContainsKey("TimeOut"))
                m_TimeOut = new TimeSpan(Convert.ToInt32(args["TimeOut"] as string));

            if (args.ContainsKey("Mock"))
                m_Mock = bool.Parse(args["Mock"].ToString());
        }

        /// <summary>
        /// Send list of object
        /// </summary>
        /// <param name="sensorMessages"> List or Sensor Messages </param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Task SendAsync(IList<object> sensorMessages,
            Action<IList<object>> onSuccess = null,
            Action<IList<IotApiException>> onError = null,
            Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Send sensor message
        /// </summary>
        /// <param name="sensorMessage"> Sensor message </param>
        /// <param name="onSuccess"> Sensor feedback </param>
        /// <param name="onError"> Error while sending message to sensor </param>
        /// <param name="args"> Arguments </param>
        /// <returns></returns>
        public async Task SendAsync(object sensorMessage, 
            Action<object> onSuccess = null, 
            Action<IotApiException> onError = null, 
            Dictionary<string, object> args = null)
        {

            XmlRpcProxy proxy = new XmlRpcProxy(m_Uri, m_TimeOut, m_Mock);

            await Task.Run(() =>
             {
                 try
                 {

                     var sensorMsg = new Message(sensorMessage).GetBody<MethodCall>();

                     var requestStr = proxy.SerializeRequest(sensorMsg);
                     var res = proxy.SendRequest(requestStr).Result;
                     var response = proxy.DeserializeResponse(res);

                     onSuccess?.Invoke(response);

                 }
                 catch (XmlRpcFaultException faultEx)
                 {
                     onError?.Invoke(new IotApiException(faultEx.Message, faultEx));
                 }
             });
        }
    }
}
