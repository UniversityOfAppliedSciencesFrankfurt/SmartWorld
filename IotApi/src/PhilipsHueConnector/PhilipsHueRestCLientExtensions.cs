using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Iot.PhilipsHueConnector;

namespace Iot
{
    public static class PhilipsHueRestCLientExtensions
    {

        public static IotApi UsePhilpsQueueRest(this IotApi api, string gatewayUrl, string userName)
        {
            PhilipsHueRestClient hue = new PhilipsHueRestClient(userName, gatewayUrl);
            api.RegisterModule(hue);
            return api;
        }

        /// <summary>
        /// Connects to gateway and generates the username, which will be used in all subsequent calls.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="gatewayUri"></param>
        /// <param name="retries"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static string GenerateUserName(this IotApi api, string gatewayUri, int retries = 3, int delay = 5000)
        {
            while (true)
            {
                var http = PhilipsHueRestClient.GetHttpClient(gatewayUri);
                var st = $"{{\"devicetype\": \"{new Random().Next()}\" }}";
              
                try
                {
                    while (true)
                    {
                        StringContent content = new StringContent(st);

                        var result = http.PostAsync("api", content).Result;
                        if (result.StatusCode != System.Net.HttpStatusCode.OK)
                            PhilipsHueRestClient.Throw(result);
                        else
                        {
                            var res = result.Content.ReadAsStringAsync().Result;
                            var gtwResult = JsonConvert.DeserializeObject<JArray>(res);

                            var err = PhilipsHueRestClient.LookupValue(gtwResult, "error");
                            if (err != null)
                            {
                                if (--retries > 0)
                                    Task.Delay(delay).Wait();
                                else
                                    throw new IotApiException($"{err}");
                            }
                            else
                            {
                                dynamic keyToken = PhilipsHueRestClient.LookupValue(gtwResult, "success");

                                return keyToken.username.Value;
                            }
                        }                       
                    }
                }
                catch (Exception ex)
                {
                    if (--retries > 0)
                        Task.Delay(delay).Wait();
                    else
                        throw ex;
                }
            }

            throw new Exception("");
        }

        //private static JToken lookupValue(JArray arr, string propName)
        //{
        //    foreach (var item in arr.Children<JObject>())
        //    {
        //        foreach (JProperty prop in item.Properties())
        //        {
        //            if (prop.Name.ToLower() == propName)
        //            {   
        //                return prop.Value;
        //            }
        //        }
        //    }

        //    return null;
        //}

        //private static JToken lookupError(JArray arr)
        //{
        //    foreach (var item in arr.Children<JObject>())
        //    {
        //        foreach (JProperty prop in item.Properties())
        //        {
        //            if (prop.Name.ToLower() == "error")
        //            {
        //                dynamic val = prop.Value;
        //                var code = val.type.Value;
        //                return prop.Value;
        //            }
        //        }
        //    }

        //    return null;
        //}




    }
}
