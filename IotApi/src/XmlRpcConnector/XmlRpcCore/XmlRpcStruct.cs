using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Defines XML Struct object which contains list of StructMember objects
    /// </summary>
    public class XmlRpcStruct
    {
        public StructMember[] Member { get; set; }
    }
}
