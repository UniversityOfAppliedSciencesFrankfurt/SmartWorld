using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot;

namespace ProfinetConnectorUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var api = getIotApi();

            api.SendAsync("",(succ)=> {
            },(err,mgs)=> {

            }).Wait();
        }

        [TestMethod]
        public void TestMethod2()
        {
            var api = new IotApi();
            api.RegisterModule(new DummyConnectorThree());
            api.Open();

            var result = api.ReceiveAsync().Result;
        }

        private IotApi getIotApi()
        {
            var api = new IotApi();
            api.RegisterModule(new DummyConnectorOne());
            api.RegisterModule(new DummyConnectorTwo());
            api.Open();
            return api;
        }
    }
}
