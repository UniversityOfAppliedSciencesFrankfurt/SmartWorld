using System;
using System.Collections.Generic;
using System.Text;
using Iot;

namespace MQTTSnClient
{
    public static class MQTTSnClientExtensions
    {

        public static IotApi UseMQTTSnClient(this IotApi api, string ip, int port)
        {
            api.RegisterModule(new MQTTSnConnector(ip, port));
            return api;
        }
    }
}
