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

        public IotApiException(string message, Exception innerException, IList<object> relatedMessages) : base(message, innerException)
        {
            this.ReceivedMessages = relatedMessages;
        }

        public IotApiException(string message,  IList<object> relatedMessages) : base(message)
        {
            this.ReceivedMessages = relatedMessages;
        }

        public IotApiException(string message, object relatedMessage) : base(message)
        {
            this.ReceivedMessages = new List<object> { relatedMessage };
        }
    }
}
