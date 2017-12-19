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
        /// XmlRpc contractor
        /// </summary>
        /// <param name="uri">Uri of CCU</param>
        public XmlRpc(string uri)
        {
            this.m_Uri = new Uri(uri);
        }

        public void Open(Dictionary<string, object> args)
        {
            if (args != null)
            {
                if (args.ContainsKey("TimeOut"))
                    m_TimeOut = new TimeSpan(Convert.ToInt32(args["TimeOut"] as string));

                if (args.ContainsKey("Mock"))
                    m_Mock = bool.Parse(args["Mock"].ToString());
            }
        }

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)

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

                    onSuccess?.Invoke(new List<object> { response });
                }
                catch (XmlRpcFaultException faultEx)
                {
                    onError?.Invoke(faultEx);
                }
            });
        }

        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            try
            {
                List<object> onSuccList = new List<object>();
                List<IotApiException> onErrList = new List<IotApiException>();

                foreach (var msg in sensorMessages)
                {
                    await this.SendAsync(sensorMessages, (sucMgs) =>
                    {
                        onSuccList.Add(sucMgs);
                    },
                    (err) =>
                    {
                        onErrList.Add(err);
                    },
                    args);
                }

                onSuccess?.Invoke(onSuccList);

                if (onErrList != null)
                {
                    onError?.Invoke(onErrList);
                    return;
                }

            }
            catch (Exception ex)
            {
                onError?.Invoke(new List<IotApiException> { (IotApiException)ex });
            }
        }
    }
}

