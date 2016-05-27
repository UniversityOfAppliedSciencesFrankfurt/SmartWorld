using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace XmlRpcLib
{
    public class DeviceDesription
    {
        public class DeviceDescription
        {
            [XmlRpcMember("TYPE")]
            public string Type
            {
                get;
                set;
            }

            [XmlRpcMember("ADDRESS")]
            public string Address
            {
                get;
                set;
            }

            [XmlRpcMissingMapping(MappingAction.Ignore), XmlRpcMember("CHILDREN")]
            public string[] Children
            {
                get;
                set;
            }
            [XmlRpcMember("PARENT")]
            public string Parent
            {
                get;
                set;
            }

            public bool IsDevice()
            {
                return string.IsNullOrEmpty(this.Parent);
            }
        }
    }
}
