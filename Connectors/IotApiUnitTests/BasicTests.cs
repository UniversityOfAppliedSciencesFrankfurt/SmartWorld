using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Daenet.Iot;
using Daenet.IoTApi;
using System.Collections.Generic;
using Daenet.IoT.Services;

namespace IotApiUnitTests
{
    [TestClass]
    public class BasicTests
    {
        public IIotApi getConnector(bool simulateErrorOnSend = false)
        {
            return new SampleConnector(simulateErrorOnSend);

        }
        [TestMethod]
        [ExpectedException(typeof(IotApiException))]
        public void TestInitialization()
        {
            IotApi api = new IotApi();
            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
              (msgs) =>
              {

              },
              (msgs, err) =>
              {

              },
              null).Wait();
            ;
        }

        [TestMethod]
        public void TestSend()
        {
            IotApi api = new IotApi();

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (msgs) =>
                {
                    Assert.IsTrue(msgs.Count == 1);
                   // Assert.IsTrue(((dynamic)msgs[0]).Prop1 == 1.2);
                },
                (msgs, err) =>
                {
                    throw err;
                },
                null).Wait();
        }

        [TestMethod]
        public void TestBatchSend()
        {
            IotApi api = new IotApi();

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.SendAsync(new List<object>()
            {  new { Prop1 = 1.2, Prop2 = ":)" } , new { Prop1 = 2.3, Prop2 = ":(" } },
                (msgs) =>
                {
                    Assert.IsTrue(msgs.Count == 2);
                  
                    //Assert.IsTrue(((dynamic)msgs[0]).Prop1 == 1.2);
                    //Assert.IsTrue(((dynamic)msgs[2]).Prop2 == ":(");
                },
                (msgs, err) =>
                {
                    throw err;
                },
                null).Wait();
        }

        [TestMethod]
        public void TestSendWithRetry()
        {
            IotApi api = new IotApi();
            api
            .RegisterRetry(2, TimeSpan.FromSeconds(1))
            .RegisterPersist(new Dictionary<string, object>())
            .RegisterModule(getConnector(true));
              

           // api.RegisterModule((anew RetryModule()).Open(null, null, null));
            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (msgs) =>
                {
                    Assert.IsTrue(msgs.Count == 1);
                    // Assert.IsTrue(((dynamic)msgs[0]).Prop1 == 1.2);
                },
                (msgs, err) =>
                {
                    throw err;
                },
                null).Wait();
        }

        [TestMethod]
        public void TestReceive()
        {
            IotApi api = new IotApi();

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.ReceiveAsync(
                (msg) =>
                {
                    Assert.IsTrue(msg != null);
                    return true;
                    
                },
                (err) =>
                {
                    return false;
                }).Wait();
        }

        private void testMethod(int a)
        {

        }

        public void go(dynamic a, dynamic b)
        {

        }
    }
}
