using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iot;

namespace XmlRpcCore
{
    /// <summary>
    /// Throws XMLRPC Exception in case of error.
    /// </summary>
    public class XmlRpcFaultException : IotApiException
    {
        /// <summary>
        /// Fault Code number
        /// </summary>
        private int m_faultCode;

        /// <summary>
        /// Fault message
        /// </summary>
        private string m_faultString;

        public int FaultCode
        {
            get
            {
                return this.m_faultCode;
            }
        }

        public string FaultString
        {
            get
            {
                return this.m_faultString;
            }
        }

        /// <summary>
        /// Returns an Fault Exception from server
        /// </summary>
        /// <param name="faultCode">Fault Code Number</param>
        /// <param name="faultString">Fault Message</param>
        public XmlRpcFaultException(int faultCode, string faultString) : base("Server returned a fault exception: [" + faultCode.ToString() + "] " + faultString)
        {
            this.m_faultCode = faultCode;
            this.m_faultString = faultString;
        }
    }
}
