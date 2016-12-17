using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.Iot
{
    public class IotApiException : Exception
    {
        public IList<object> ReceivedMessages { get; protected set; }

        public IotApiException(string message) : base(message)
        {

        }

        public IotApiException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public IotApiException(string message, Exception innerException, IList<object> receivedMessages) : base(message, innerException)
        {

        }
    }
}
