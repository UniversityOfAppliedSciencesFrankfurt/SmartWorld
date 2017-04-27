using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iot.PhilipsHueConnector.Entities
{
    public class Schedule
    {
        public string name { get; set; }
        public string description { get; set; }
        public Command command { get; set; }
        public string localtime { get; set; }
        public bool autodelete { get; set; }
        public bool recycle { get; set; }
        public bool time { get; set; }
    }
    public class Command
    {
        public  string address { get; set; }
        public  string method { get; set; }
        public body body { get; set; }
    }
   public class body
    {
        public bool On;
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
