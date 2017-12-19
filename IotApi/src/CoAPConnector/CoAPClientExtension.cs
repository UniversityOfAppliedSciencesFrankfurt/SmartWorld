using Iot;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoAPConnector
{
    public static class CoAPClientExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static IotApi UserCoAPModule(this IotApi api, ICoapEndpoint endPoint)
        {
            api.RegisterModule(new CoAPclientConnector(endPoint));
            return api;
        }
    }
}
