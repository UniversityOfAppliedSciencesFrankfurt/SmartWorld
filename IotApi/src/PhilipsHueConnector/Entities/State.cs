using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{
    public class State
    {
        public bool On { get; set; }
        public int Bri { get; set; }
        public int Hue { get; set; }
        public int Sat { get; set; }
        public string Effect { get; set; }
        public List<double> xy { get; set; }
        public int ct { get; set; }
        public string alert { get; set; }
        public string colormode { get; set; }
        public bool reachable { get; set; }

        public string ToLightStateJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("{");
            if (this.On)
                sb.AppendLine($"\"on\" : true");
            else
                sb.AppendLine($"\"on\" : false");

            if (this.On)
            {
                sb.AppendLine(",");
                sb.AppendLine($"\"bri\" : {this.Bri},");
                sb.AppendLine($"\"hue\" : {this.Hue},");
                sb.AppendLine($"\"sat\" : {this.Sat},");

                if (this.Effect == null)
                    sb.AppendLine($"\"effect\" : \"none\"");
                else
                    sb.AppendLine($",\"effect\" : \"{this.Effect}\"");

                if (this.xy != null)
                {
                    sb.Append(",");
                    sb.AppendLine($"\"xy\" : ");
                    sb.AppendLine(JsonConvert.SerializeObject(this.xy));
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }

}
