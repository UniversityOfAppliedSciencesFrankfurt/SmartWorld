using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CcuLib
{
    /// <summary>
    /// Device Family list
    /// </summary>
    [DataContract]
    public enum DeviceFamily
    {
        [EnumMember]
        HomeMaticBidCoS,

        [EnumMember]
        HomeMaticWired,

        [EnumMember]
        Max
    }
}
