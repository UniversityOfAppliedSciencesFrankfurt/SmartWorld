using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector
{

    public class HueCommand
    {
        internal string Method { get; set; } = "get";
    }


    public class GetConfig : HueCommand
    {
         internal string Path { get; set; } = "config";
    }

    public class GetLights : HueCommand
    {
        internal string Path { get; set; } = "ligths";
    }

    public class GetSensors : HueCommand
    {
        internal string Path { get; set; } = "sensors";
    }

    public class GetGroups : HueCommand
    {
        internal string Path { get; set; } = "groups";
    }

    public class GetScenes : HueCommand
    {
        internal string Path { get; set; } = "scenes";
    }

    public class GetRules : HueCommand
    {
        internal string Path { get; set; } = "rules";
    }
}
