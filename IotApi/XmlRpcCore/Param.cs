using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlRpcCore
{
    /// <summary>
    /// Defines global object named "Param" which contains the value of parameter.
    /// </summary>
    public class Param
    {
        /// <summary>
        /// Value of parameter
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Converts value into string type
        /// </summary>
        /// <returns>string type of given value</returns>
        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
