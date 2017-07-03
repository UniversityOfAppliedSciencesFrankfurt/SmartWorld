using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{
    public class Group
    {
        public string[] lights { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public bool on { get; set; }
        public int hue { get; set; }
        public string effect { get; set; }
    }
}
