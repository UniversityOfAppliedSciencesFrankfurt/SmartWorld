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
        private ISendModule m_NextSendModule;

        private string m_UserName;

        private string m_GatewayUrl;

        private string m_ApiSuffix = "api";

        public PhilipsHueRestClient()
        {

        }

        public PhilipsHueRestClient(string userName, string gatewayUrl)
        {
            this.m_UserName = userName;

            this.m_GatewayUrl = gatewayUrl;
        }

        public ISendModule NextSendModule
        {
            get
            {
                return m_NextSendModule;
            }

            set
            {
                m_NextSendModule = value;
            }
        }

        public void Open(Dictionary<string, object> args)
        {
            if (m_UserName == null || m_GatewayUrl == null)
                throw new Exception("Username or gateay URL not specified.");
        }

        public Task SendAsync(IList<object> sensorMessages, 
            Action<IList<object>> onSuccess = null, 
            Action<IList<IotApiException>> onError = null, 
            Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        internal static HttpClient GetHttpClient(string gatewayUri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gatewayUri);

            return client;
        }

        internal static void Throw(HttpResponseMessage response)
        {
            throw new Exception($"{response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
        }
    }
}
