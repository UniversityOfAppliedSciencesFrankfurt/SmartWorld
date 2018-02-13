using System;
using System.Collections.Generic;
using System.Text;
using Iot;

namespace ProfinetConnector
{
    public static class ProfinetConnectorExtension
    {
        /// <summary>
        /// IotApi extension 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static IotApi UseProfinet(this IotApi api, string connStr)
        {
            api.RegisterModule(new ProfinetConnector(connStr));
            return api;
        }
    }
}
