using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;
using OpcUAConnector;
using Xunit;

namespace OpcUAUnitTests
{
    public class OpcUATest
    {
        [Fact(DisplayName ="OpenTest")]
        public void OpenTest()
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            arg.Add("endpoint", "opc.tcp://myserver:51210/UA/SampleServer");
            
            IotApi api = new IotApi();
            api.RegisterModule(new OPCConnector());
            api.Open(arg);
            api.SendAsync("fff").Wait();
        }            
    }
}
