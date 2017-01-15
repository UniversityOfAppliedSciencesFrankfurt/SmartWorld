using Iot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PhilipsHueConnector.Entities;
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

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            HttpResponseMessage response = await executeMsg(sensorMessage);

            var str = await response.Content.ReadAsStringAsync();

            object res = JsonConvert.DeserializeObject(str);

            if (res is JArray)
            {
                var result = JsonConvert.DeserializeObject<JArray>(str);
                GatewayError err = null;

                if (isError(result, out err))
                {
                    onError?.Invoke(new IotApiException(":( An error has ocurred!", err));
                }
                else
                {
                    throw new IotApiException("Do not know meaning of this response", result);
                }
            }
            else if (res is JObject)
            {
                List<Device> devices = new List<Entities.Device>();

                foreach (var prop in ((JObject)res).Properties())
                {
                    var dev = JsonConvert.DeserializeObject<Device>(prop.Value.ToString());
                    dev.Id = prop.Name;
                    devices.Add(dev);
                }

                onSuccess?.Invoke(devices);
            }
        }

        private async Task<HttpResponseMessage> executeMsg(object sensorMessage)
        {
            HueCommand cmd = sensorMessage as HueCommand;
            if (cmd == null)
                throw new IotApiException("Unknown command specified.");

            HttpResponseMessage response = null;

            var httpClient = GetHttpClient(m_GatewayUrl);
            if (cmd.Method == "get")
            {
                response = await httpClient.GetAsync(getUri(cmd));
            }
            else if (cmd.Method == "post")
            {
                //httpClient.GetAsync(getUri(cmd));
            }
            else if (cmd.Method == "put")
            {
                string data = ((SetCommandBase)sensorMessage).State.ToLightStateJson();
                //data = JsonConvert.SerializeObject(new { on = true, bri = 100 });
                StringContent content = new StringContent(data);
                string uri = getUri(cmd);
                response = await httpClient.PutAsync(uri, content);
            }

            return response;
        }
    
        private bool isError(JArray result, out GatewayError err)
        {
            var jToken = LookupValue(result, "error");
            if (jToken == null)
            {
                err = null;
                return false;
            }

            err = JsonConvert.DeserializeObject<GatewayError>(jToken.ToString());
            return jToken != null;
        }

        private string getUri(HueCommand cmd)
        {
            return $"/{m_ApiSuffix}/{m_UserName}/{cmd.Path}";
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

        internal static JToken LookupValue(JArray arr, string propName)
        {
            foreach (var item in arr.Children<JObject>())
            {
                foreach (JProperty prop in item.Properties())
                {
                    if (prop.Name.ToLower() == propName)
                    {
                        return prop.Value;
                    }
                }
            }

            return null;
        }
    }
}
