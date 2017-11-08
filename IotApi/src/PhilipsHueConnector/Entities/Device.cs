using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{
    public class Device
    {
        public State State { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string Modelid { get; set; }
        public string Manufacturername { get; set; }
        public string Uniqueid { get; set; }
        public string Swversion { get; set; }
        public string Swconfigid { get; set; }
        public string Productid { get; set; }

        public string Id { get; set; }
    }
}
