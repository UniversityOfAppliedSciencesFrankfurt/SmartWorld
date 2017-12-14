using Iot;
using System;
using System.Collections.Generic;
using System.Text;

namespace XmlRpcCore
{
    public static class XmlRpcExtensions
    {
        /// <summary>
        /// Calling XmlRpc connector 
        /// </summary>
        /// <param name="api">IotApi object</param>
        /// <param name="uri">CCU url</param>
        public static IotApi UseXmlRpc(this IotApi api, string uri)
        {
            return api.RegisterModule(new XmlRpc(uri));
        }
    }
}
