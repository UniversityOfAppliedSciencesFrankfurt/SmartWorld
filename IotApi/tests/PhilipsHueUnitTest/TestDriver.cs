using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhilipsHueUnitTest
{
    /// <summary>
    /// Use this class to set values, which correspond to you renvironment.
    /// Hen all set exactly for your environmant, tests should pass.
    /// </summary>
    internal class TestDriver
    {
        /// <summary>
        /// Number of connected devices on Hue Gateway.
        /// </summary>
        public const int NumOfDevices = 4;

        /// <summary>
        /// The identifier of the light, which will be used for lighting tests.
        /// </summary>
        public const string LightStateReferenceId = "1";
    }
}
