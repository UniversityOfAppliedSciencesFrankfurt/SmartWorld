using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector
{

    public abstract class HueCommand
    {
        internal string Method { get; set; } = "get";

        internal abstract string Path {get;set;}
    }


    public class GetConfig : HueCommand
    {
         internal override string Path { get; set; } = "config";
    }

    public class GetLights : HueCommand
    {
        internal override string Path { get; set; } = "lights";
    }

    public class GetSensors : HueCommand
    {
        internal override string Path { get; set; } = "sensors";
    }

    public class GetGroups : HueCommand
    {
        internal override string Path { get; set; } = "groups";
    }

    public class GetScenes : HueCommand
    {
        internal override string Path { get; set; } = "scenes";
    }

    public class GetRules : HueCommand
    {
        internal override string Path { get; set; } = "rules";
    }
}
