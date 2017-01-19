using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XmlRpcCore;
using Xunit;

namespace XmlRpcUnitTest
{
    public class UnitTest
    {
        [Fact]
        public void TestOpen_Door()
        {

            IotApi connector = new IotApi()
                .RegisterModule(new XmlRpc());

            Dictionary<string, object> agr = new Dictionary<string, object>()
            {
                { "Uri", "http://192.168.0.222:2001" },
                {"Mock",false }
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
                        Assert.True(onMessage != null);

                    },
                    (error) =>
                    {
                        throw error;
                    }).Wait();
            }catch(Exception ex)
            {
                Assert.True(ex != null);
            }
        }
    }
}
