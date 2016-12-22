using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XmlRpcCore;
using Xunit;

namespace IotApiTests
{
    public class XmlRpcTests
    {
        [Fact]
        public void Open_Door()
        {
            // XmlRpcConnector.XmlRpcConnector connector = new XmlRpcConnector.XmlRpcConnector();
            IotApi connector = new IotApi()
                .RegisterModule(new XmlRpc());
            // IIotApi connector = new XmlRpcConnector.XmlRpcConnector();
            Dictionary<string, object> agr = new Dictionary<string, object>()
            {
                { "Uri", "http://192.168.0.222:2001" },
                {"Mock",true }
            };
            connector.Open(agr);

            MethodCall request = new MethodCall()
            {

                MethodName = "setValue",
                SendParams = new List<Param>()
                 {
                     new Param()
                     {
                        Value = (string)"LEQ1335713:1",
                     },
                     new Param()
                     {
                        Value = (string)"STATE",
                     },
                     new Param()
                     {
                        Value = (bool)true,
                     },
                 }
            };

            try
            {
                connector.SendAsync(request,
                    (onMessage) =>
                    {
                        foreach (var mgs in onMessage)
                        {
                            MethodResponse res = mgs as MethodResponse;
                            Assert.True(res != null);
                        }
                    },
                    (msgss, err) =>
                    {
                        throw err;
                    }).Wait();
            }catch(Exception e)
            {

            }
        }
    }
}
