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
        //public string time { get; set; }
    }
    public class Command
    {
        public  string address { get; set; }
        public  string method { get; set; }
        public body body { get; set; }
    }
   public class body
    {
        public bool on;
        public int bri;
    }
}
