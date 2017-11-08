using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Defines MethodResponse object which contains list of received parameters from server.
    /// </summary>
    public class MethodResponse
    {
        /// <summary>
        /// List of responsed parameters
        /// </summary>
        public List<Param> ReceiveParams { get; set; }

        /// <summary>
        /// check whether the Response is type of MethodResponse
        /// </summary>
        /// <param name="obj">Object needs to be checked</param>
        /// <returns></returns>
        public static bool isMethodResponse(object obj)
        {
            if (obj.GetType() == typeof(MethodResponse)) return true;
            else return false;
        }
    }
}
