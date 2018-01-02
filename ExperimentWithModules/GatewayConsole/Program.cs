using Daenet.Azure.Devices.Gateway;
using Daenet.IotGateway.Common.Logger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace GatewayConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var p = $"{AppDomain.CurrentDomain.BaseDirectory}GatewayConfig.json";
            var path = $"{Directory.GetCurrentDirectory()}\\GatewayConfig.json";
            var config = JsonConvert.DeserializeObject<GatewayConfiguration>(File.ReadAllText(p));
            var gw1 = GatewayInterop.CreateFromConfig(config, new List<IGatewayLogger>() { new Logger() });
        }
    }
}
