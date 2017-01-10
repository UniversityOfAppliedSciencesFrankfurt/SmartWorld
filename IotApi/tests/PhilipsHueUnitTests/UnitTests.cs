using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PhilipsHueConnector;

namespace PhilipsHueUnitTests
{
    public class UnitTests
    {
        private string m_GtwIP = "";

        [Fact]
        public void GenerateKeyTest()
        {
            var key = new IotApi().GenerateKey(m_GtwIP);
        }
    }
}
