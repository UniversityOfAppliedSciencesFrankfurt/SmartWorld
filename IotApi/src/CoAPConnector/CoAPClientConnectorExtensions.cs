using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;

namespace CoAPConnector
{
    public  class CoAPClientConnectorExtensions : IInjectableModule
    {
        //ICoapEndpoint endpoint;

        //public static IotApi Listen (this IotApi api)
        //{
        //    CoapClient client = new CoapClient(endpoint);
        //    client.listen();
        //    return null;

        //}
        public void Open(Dictionary<string, object> args)
        {
            
        }

        public string Se(string n)
        {
            return n;
        }
    }
}
