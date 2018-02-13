using System;
using System.Collections.Generic;
using System.Text;
using Iot;

namespace ProfinetConnector
{
    public class ProfinetException : IotApiException
    {
        public ProfinetException(string errDesc) : base(errDesc)
        {
        }
    }
}
