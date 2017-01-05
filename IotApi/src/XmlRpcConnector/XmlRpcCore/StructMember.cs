using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Define StructMember object which contains properties of "name" and "value".
    /// </summary>
    public class StructMember
    {
        /// <summary>
        /// Name of Struct object
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of Struct object
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Creates Struct Object
        /// </summary>
        /// <param name="name">Name of Struct object</param>
        /// <param name="value">Value of struct object</param>
        public StructMember(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public StructMember()
        {
        }
    }
}
