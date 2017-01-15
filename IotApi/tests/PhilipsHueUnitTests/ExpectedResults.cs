using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueUnitTests
{
    /// <summary>
    /// Use this class to set values, which correspond to you renvironment.
    /// Hen all set exactly for your environmant, tests should pass.
    /// </summary>
    internal class ExpectedResults
    {
        /// <summary>
        /// Number of connected devices on Hue Gateway.
        /// </summary>
        public const int NumOfDevices = 5;
    }
}
