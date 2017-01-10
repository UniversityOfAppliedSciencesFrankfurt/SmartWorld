using Iot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipsHueConnector
{
    public class PhilipsHueRestClient : ISendModule
    {
        private string m_Key;

        public PhilipsHueRestClient()
        {

        }

        public PhilipsHueRestClient(string gatewayKey)
        {
            this.m_Key = gatewayKey;
        }

        public ISendModule NextSendModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Open(Dictionary<string, object> args)
        {
            if (args != null && args.ContainsKey("Key"))
            {
                m_Key = args["key"] as string;
            }
            else
                throw new Exception("Key is not specified. Please provide a 'key' in argument list.");
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        internal static HttpClient GetHttpClient(string gatewayUri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(Path.Combine(gatewayUri, "api"));

            return client;
        }
    }
}
