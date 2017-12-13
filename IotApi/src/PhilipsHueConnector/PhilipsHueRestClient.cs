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
        /// <summary>
        /// Next module object
        /// </summary>
        private ISendModule m_NextSendModule;
        /// <summary>
        /// Philips hue gateway user name
        /// </summary>
        private string m_UserName;
        /// <summary>
        /// Philips hue gateway url
        /// </summary>
        private string m_GatewayUrl;
        /// <summary>
        /// Philips hue api suffix
        /// </summary>
        private string m_ApiSuffix = "api";

        /// <summary>
        /// Contractor of PhilipsHueRestClient
        /// </summary>
        /// <param name="userName"> Gateway user name</param>
        /// <param name="gatewayUrl"> Gateway url</param>
        public PhilipsHueRestClient(string userName, string gatewayUrl)
        {
            this.m_UserName = userName;

            this.m_GatewayUrl = gatewayUrl;
        }

        /// <summary>
        /// Property of next module
        /// </summary>
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

        /// <summary>
        /// Establish a connection with Philips hue gateway
        /// </summary>
        /// <param name="args"></param>
        public void Open(Dictionary<string, object> args)
        {
            if (m_UserName == null || m_GatewayUrl == null)
                throw new Exception("Username or gateay URL not specified.");
        }

        /// <summary>
        /// Send command to Philips hue devices
        /// </summary>
        /// <param name="sensorMessages">Collection of commands</param>
        /// <param name="onSuccess">Success responses from Philips hue device</param>
        /// <param name="onError">Error messages from Philips hue device</param>
        /// <param name="args">Arguments but in Philips hue did not use</param>
        /// <returns></returns>
        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null,
                               Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            List<object> succList = new List<object>();
            List<IotApiException> errList = new List<IotApiException>();

            foreach (var mgs in sensorMessages)
            {
                await this.SendAsync(mgs, (suc) =>
                 {
                     succList.Add(suc);
                 }, (err) =>
                 {
                     errList.Add(err);
                 }, args);
            }

            onSuccess?.Invoke(succList);
            onError?.Invoke(errList);
        }

        /// <summary>
        /// Send command to Philips hue device
        /// </summary>
        /// <param name="sensorMessage">Command to send</param>
        /// <param name="onSuccess">Success message from Philips hue device</param>
        /// <param name="onError">Error message from Philips hue device</param>
        /// <param name="args">Arguments but in Philips hue did not use</param>
        /// <returns></returns>
        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null,
                                Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            HttpResponseMessage response = await executeMsg(sensorMessage);

            var str = await response.Content.ReadAsStringAsync();

            object res = JsonConvert.DeserializeObject(str);

            if (res is JArray)
            {
                var result = JsonConvert.DeserializeObject<JArray>(str);
                GatewayError err = null;

                if (isError(result, ref err))
                {
                    onError?.Invoke(new IotApiException(":( An error has ocurred!", err));

                }
                else if (isSuccess(result))
                {
                    onSuccess?.Invoke(result);
                }
                else
                {
                    throw new IotApiException("Do not know meaning of this response", result);
                }
            }
            else if (res is JObject)
            {
                var devices = getDevices(sensorMessage, res);

                onSuccess?.Invoke(devices);
            }

        }

        /// <summary>
        /// Get device/lights list 
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <param name="res"></param>
        /// <returns></returns>
        private object getDevices(object sensorMessage, object res)
        {
            if (sensorMessage is GetLightStates)
            {
                return JsonConvert.DeserializeObject<Device>(((JObject)res).Root.ToString());
            }
            else if (sensorMessage is GetLights)
            {
                List<Device> devices = new List<Entities.Device>();

                foreach (var prop in ((JObject)res).Properties())
                {
                    try
                    {
                        Device dev = JsonConvert.DeserializeObject<Device>(prop.Value.ToString());
                        dev.Id = prop.Name;
                        devices.Add(dev);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return devices;
            }

            throw new IotApiException("Device not fount");
        }

        /// <summary>
        /// Command execute with HttpClient 
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <returns></returns>
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
                StringContent str = new StringContent(JsonConvert.SerializeObject(cmd.Body, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore }));
                response = await httpClient.PostAsync(getUri(cmd), str);
            }
            else if (cmd.Method == "put")
            {
                var sta = JsonConvert.SerializeObject(cmd.Body, new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore });

                StringContent str = new StringContent(sta);
                response = await httpClient.PutAsync(getUri(cmd), str);
            }
            else
            {
                throw new IotApiException("HueCommand method is not assigned.");
            }

            return response;
        }

        /// <summary>
        /// Check error is occurred or not 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="err"></param>
        /// <returns></returns>
        private bool isError(JArray result, ref GatewayError err)
        {
            var jToken = LookupValue(result, "error");

            if (jToken != null)
            {
                err = JsonConvert.DeserializeObject<GatewayError>(jToken.ToString());
                return true;
            }

            return false;
        }

        /// <summary>
        /// Look for success message
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool isSuccess(JArray result)
        {
            var jToken = LookupValue(result, "success");
            return jToken != null;
        }

        /// <summary>
        /// Get uri of api
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private string getUri(HueCommand cmd)
        {
            return $"/{m_ApiSuffix}/{m_UserName}/{cmd.Path}";
        }

        /// <summary>
        /// Get Http client
        /// </summary>
        /// <param name="gatewayUri"></param>
        /// <returns></returns>
        internal static HttpClient GetHttpClient(string gatewayUri)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(gatewayUri);

            return client;
        }

        /// <summary>
        /// Throw an exception
        /// </summary>
        /// <param name="response"></param>
        internal static void Throw(HttpResponseMessage response)
        {
            throw new Exception($"{response.StatusCode} - {response.Content.ReadAsStringAsync().Result}");
        }

        /// <summary>
        /// Look up value in JArray
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
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
