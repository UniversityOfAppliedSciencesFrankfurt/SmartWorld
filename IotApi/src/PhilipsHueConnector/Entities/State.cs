using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{
    public class State
    {
        public bool? on { get; set; }
        public int? bri { get; set; }
        public int? hue { get; set; }
        public int? sat { get; set; }
        public string effect { get; set; }
        public List<double> xy { get; set; }
        public int? ct { get; set; }
        public string alert { get; set; }
        public string colormode { get; set; }
        public bool? reachable { get; set; }
    }

}
