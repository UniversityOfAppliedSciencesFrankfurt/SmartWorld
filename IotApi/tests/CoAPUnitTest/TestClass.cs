using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoAPConnector;
using Moq;
using Xunit;

namespace CoAPUnitTest
{
    public class TestClass
    {
        [Fact]
        public void Test()
        {
            IotApi api = new IotApi()
                .RegisterModule(new CoAPConnector.CoAPConnector());

            Dictionary<string, object> agr = new Dictionary<string, object>();
            agr.Add("endPoint", new Mock<ICoapEndpoint>());

            api.Open(agr);

            api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Get
            }).Wait();
        }
    }
}
