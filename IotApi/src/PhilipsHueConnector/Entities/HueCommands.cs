using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueConnector.Entities
{

    public abstract class HueCommand
    {
        /// <summary>
        /// HTTP method used in REST operation.
        /// </summary>
        internal abstract string Method { get; set; }

        /// <summary>
        /// REST path of the resource.
        /// </summary>
        internal abstract string Path {get;set;}
    }

    public class GetCommandBase : HueCommand
    {
        internal override string Method { get; set; } = "get";

        internal override string Path { get; set; } 
    }

    public class SetCommandBase : HueCommand
    {
        internal override string Method { get; set; } = "put";

        internal override string Path { get; set; }

        /// <summary>
        /// Identifier of device. USed in PU/POST operations only.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Used by PUT/POST commands only.
        /// </summary>
        public State State { get; set; }
    }

    #region Get Commands

    public class GetConfig : GetCommandBase
    {
         internal override string Path { get; set; } = "config";
    }

    public class GetLights : GetCommandBase
    {
        internal override string Path { get; set; } = "lights";
    }


    public class GetSensors : GetCommandBase
    {
        internal override string Path { get; set; } = "sensors";
    }

    public class GetGroups : GetCommandBase
    {
        internal override string Path { get; set; } = "groups";
    }

    public class GetScenes : GetCommandBase
    {
        internal override string Path { get; set; } = "scenes";
    }

    public class GetRules : GetCommandBase
    {
        internal override string Path { get; set; } = "rules";
    }
    #endregion

    #region Set Commands
   
    /// <summary>
    /// Sets the state of the light
    /// </summary>
    public class SetLightState : SetCommandBase
    {
        private string m_Method;

        internal override string Method
        {
            get
            {
                return "put";
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        private string m_Path;

        internal override string Path
        {
            get
            {
                return $"lights/{this.Id}/state";
            }

            set
            {
                throw new NotSupportedException();
            }
        }

    }
    #endregion
}
