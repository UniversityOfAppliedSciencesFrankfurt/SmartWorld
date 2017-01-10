using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhilipsHueConnector
{
    public static class PhilipsHueRestCLientExtensions
    { 

        public static IotApi UsePhilpsQueueRest(this IotApi api, string gatewayKey)
        {
            PhilipsHueRestClient hue = new PhilipsHueRestClient(gatewayKey);
            api.RegisterModule(hue);
            return api;
        }

        public static IotApi GenerateKey(this IotApi api, string gatewayUri )
        {
            var http = PhilipsHueRestClient.GetHttpClient(gatewayUri);
            StringContent content = new StringContent($"'deviceType': {new Random().Next()}");
            http.PostAsync("", content);
            return api;
        }
    }
}
