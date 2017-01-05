using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Defines MethodFaultResponse object which includes code number and message of Fault Message returned from server in case of occuring error.
    /// </summary>
    public class MethodFaultResponse
    {
        /// <summary>
        /// Fault Code number
        /// </summary>
        public int FaultCode { get; set; }

        /// <summary>
        /// Fault Message
        /// </summary>
        public string Message { get; set; }


        /// <summary>
        /// Checks whether the Response is a fault message
        /// </summary>
        /// <param name="obj">object needs to be checked</param>
        /// <returns></returns>
        public static bool isMethodFaultResponse(object obj)
        {
            if (obj.GetType() == typeof(MethodFaultResponse)) return true;
            else return false;
        }

    }
}
