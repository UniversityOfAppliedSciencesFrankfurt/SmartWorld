using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot
{
    /// <summary>
    /// Exception thrown inside of IotApi.
    /// </summary>
    public class IotApiException : Exception
    {
        /// <summary>
        /// List of messages related to error.
        /// </summary>
        public IList<object> ReceivedMessages { get; protected set; }

        public IotApiException(string errDesc) : base(errDesc)
        {

        }

        public IotApiException(string errDesc, Exception innerException) : base(errDesc, innerException)
        {

        }

        public IotApiException(string message, Exception innerException, IList<object> states) : base(message, innerException)
        {
            this.ReceivedMessages = states;
        }

        public IotApiException(string message,  IList<object> state) : base(message)
        {
            this.ReceivedMessages = state;
        }

        public IotApiException(string message, object state) : base(message)
        {
            this.ReceivedMessages = new List<object> { state };
        }
    }
}
