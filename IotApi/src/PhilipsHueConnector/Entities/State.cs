using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{
    public class State
    {
        public bool ON { get; set; }
        public int Bri { get; set; }
        public int Hue { get; set; }
        public int Sat { get; set; }
        public string Effect { get; set; }
        public List<double> Xy { get; set; }
        public int Ct { get; set; }
        public string Alert { get; set; }
        public string Colormode { get; set; }
        public bool Reachable { get; set; }
    }

}
