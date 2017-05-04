using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot.PhilipsHueConnector.Entities
{

    public  class HueCommand
    {

        /// <summary>
        /// HTTP method used in REST operation.
        /// </summary>
        public virtual string Method { get; set; }

        /// <summary>
        /// REST path of the resource.
        /// </summary>
        public virtual string Path {get;set;}

        /// <summary>
        /// Used when body is directly set as untyped value.
        /// </summary>
        public object Body { get; set; }
        
    }


    public class GetCommandBase : HueCommand
    {
        public override string Method { get; set; } = "get";

        public override string Path { get; set; }
    }


    public class SendCommandBase : HueCommand
    {
        public override string Method { get; set; } = "put";

        public override string Path { get; set; }

        /// <summary>
        /// Identifier of device. USed in PUT/POST operations only.
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
        public override string Path { get; set; } = "config";
    }

    public class GetLights : GetCommandBase
    {
        public override string Path { get; set; } = "lights";
    }


    public class GetSensors : GetCommandBase
    {
        public override string Path { get; set; } = "sensors";
    }

    public class GetGroups : GetCommandBase
    {
        public override string Path { get; set; } = "groups";
    }

    public class GetScenes : GetCommandBase
    {
        public override string Path { get; set; } = "scenes";
    }

    public class GetRules : GetCommandBase
    {
        public override string Path { get; set; } = "rules";
    }
    public class GetSchedules : GetCommandBase
    {
        public override string Path { get; set; } = "schedules";
    }
    #endregion

    #region Set Commands
   
    /// <summary>
    /// Sets the state of the light
    /// </summary>
    public class SetLightState : SendCommandBase
    {
        private string m_Method;

        public override string Method
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

        public override string Path
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
