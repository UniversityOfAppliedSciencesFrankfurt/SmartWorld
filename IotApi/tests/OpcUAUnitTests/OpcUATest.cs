
using System.Collections.Generic;
using Iot;
using OpcUAConnector;
using Xunit;
using Opc.Ua.Client;

namespace OpcUAUnitTests
{
    public class OpcUATest
    {
        [Fact(DisplayName = "OpenTest")]
        public void OpenTest()
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            arg.Add("endpoint", "opc.tcp://aqib:51210/UA/SampleServer");

            IotApi api = new IotApi();  
            api.RegisterModule(new OPCConnector()); 
            api.Open(arg);
            api.SendAsync("ffff").Wait();


        }
        [Fact]
        public void NodeWriteTest()
        {
            Dictionary<string, object> arg = new Dictionary<string, object>();
            arg.Add("endpoint", "opc.tcp://aqib:51210/UA/SampleServer");

            IotApi api = new IotApi();
            api.RegisterModule(new OPCConnector());
            api.Open(arg);


            var list = new List<MonitoredItem> {
                new MonitoredItem(new Subscription().DefaultItem)
                {
                    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i=2267"
                } };
            api.SendAsync(list).Wait();
            //



        }
    }
}













































