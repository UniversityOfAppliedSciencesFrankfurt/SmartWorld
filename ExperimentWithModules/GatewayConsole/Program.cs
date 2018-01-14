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
            startGateway();
            Console.WriteLine("Press any key to exit!!");
            Console.ReadKey();
        }

        private static void startGateway()
        {

            var p = $"{AppDomain.CurrentDomain.BaseDirectory}GatewayConfig.json";
            var path = $"{Directory.GetCurrentDirectory()}\\GatewayConfig.json";
            var config = JsonConvert.DeserializeObject<GatewayConfiguration>(File.ReadAllText(p));

            //while (interval > 0)
            //{
                var gw1 = GatewayInterop.CreateFromConfig(config, new List<IGatewayLogger>() { new Logger() });
            //    interval--;
            //}
        }
    }
}
