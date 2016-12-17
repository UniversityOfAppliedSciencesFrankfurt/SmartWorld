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
        public IInjectableModule getDefaultModule(bool simulateErrorOnSend = false)
        {
            return new SampleConnector(simulateErrorOnSend);
        }

        [TestMethod]
        [ExpectedException(typeof(IotApiException))]
        public void TestInit()
        {
            IotApi api = new IotApi();

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" }).Wait();
        }

        /// <summary>
        /// Test positive init with NULL-send.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(IotApiException))]
        public void TestSimpleSend()
        {
            IotApi api = new IotApi();

            api.Open();

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

        /// <summary>
        /// Test sending of the message.
        /// </summary>
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

        /// <summary>
        /// Test batched send.
        /// </summary>
        [TestMethod]
        public void TestBatchSend()
        {
            //todo.
        }

        /// <summary>
        /// Test sending of the message with retry.
        /// </summary>
        [TestMethod]
        public void TestSendWithRetry()
        {
            IotApi api = new IotApi();
            api
            .RegisterRetry(2, TimeSpan.FromSeconds(1))
            .RegisterPersist(new Dictionary<string, object>())
            .RegisterModule(getDefaultModule(true));
           
            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (msgs) =>
                {
                    Assert.IsTrue(msgs.Count == 1);
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
            IotApi api = new IotApi()
              .RegisterPersist(new Dictionary<string, object>())
              .RegisterModule(getDefaultModule(true));

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.ReceiveAsync(
                (msgs) =>
                {
                   
                },
                (msgs, err) =>
                {
                   
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
