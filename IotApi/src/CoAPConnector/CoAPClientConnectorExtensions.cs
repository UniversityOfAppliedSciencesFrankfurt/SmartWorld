using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoAPConnector
{
    public static class CoAPClientConnectorExtensions
    {
        ICoapEndpoint endpoint;

        public static IotApi Listen (this IotApi api)
        {
            CoapClient client = new CoapClient(endpoint);
            client.listen();
            return null;

        }
    }
}
