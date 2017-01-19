using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot.PhilipsHueConnector
{
    public class GatewayError
    {
        public string type { get; set; }

        public string address { get; set; }

        public string description { get; set; }

        public override string ToString()
        {
            return $"Type:{this.type}, Address:{this.address}, Description:{this.description}";
        }
    }
}
