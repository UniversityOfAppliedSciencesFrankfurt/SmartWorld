
using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IotApiTests
{

    /// <summary>
    /// Set of test which ensure core IoTApi functionality.
    /// </summary>
    public class CoreTests
    {
        /// <summary>
        /// Returns SampleProtocol as default module.
        /// </summary>
        /// <param name="simulateErrorOnSend"></param>
        /// <returns></returns>
        public IInjectableModule getDefaultModule(bool simulateErrorOnSend = false)
        {
            return new SampleProtocol(simulateErrorOnSend);
        }


        /// <summary>
        /// Makes sure that IoTApi will fail if Open is not called.
        /// </summary>
        [Fact]
        public void TestInit()
        {
            Assert.Throws(typeof(AggregateException), () =>
            {
                IotApi api = new IotApi();

                api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" }).Wait();
            });
        }


        /// <summary>
        /// Test positive init with NULL-send.
        /// </summary>
        [Fact]
        public void TestSimpleSend()
        {
            IotApi api = new IotApi();

            api.Open();

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
              (msgs) =>
              {
                  throw new InvalidOperationException("This should not be executed, because no pipeline is defined!");
              },
              (msgs, err) =>
              {
                  throw new InvalidOperationException("This should not be executed, because no pipeline is defined!");
              },
              null).Wait();
        }


        /// <summary>
        /// Test sending of the message.
        /// </summary>
        [Fact]
        public void TestSend()
        {
            IotApi api = new IotApi().
            RegisterModule(getDefaultModule(false));

            api.Open(new System.Collections.Generic.Dictionary<string, object>());
            
            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (msgs) =>
                {
                    Assert.True(msgs.Count == 1);
                    Assert.True(((dynamic)msgs[0]).Prop1 == 1.2);
                    Assert.True(((dynamic)msgs[0]).Prop2 == ":)");
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
        [Fact]
        public void TestBatchSend()
        {
            IotApi api = new IotApi().
            RegisterModule(getDefaultModule(false));

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            api.SendAsync(new List<object>
            {
                new { Prop1 = 1.23, Prop2 = ":)" },
                new { Prop1 = 1.2, Prop2 = ":):)" } },

                (msgs) =>
                {
                    Assert.True(msgs.Count == 2);
                    Assert.True(((dynamic)msgs[0]).Prop1 == 1.23);
                    Assert.True(((dynamic)msgs[1]).Prop2 == ":):)");
                },
                (msgs, err) =>
                {
                    throw err;
                },
                null).Wait();
        }


        /// <summary>
        /// Test sending of the message with retry.
        /// </summary>
        [Fact]
        public void TestSendWithRetry()
        {
            IotApi api = new IotApi()
            .RegisterRetryModule(2, TimeSpan.FromSeconds(1))
            .RegisterPersistModule(new Dictionary<string, object>())
            .RegisterModule(getDefaultModule(false));

        
            api.Open();

            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (msgs) =>
                {
                    Assert.True(msgs.Count == 1);
                    Assert.True(((dynamic)msgs[0]).Prop1 == 1.2);
                },
                (msgs, err) =>
                {
                    throw err;
                },
                null).Wait();
        }


        /// <summary>
        /// Test sending of the message with retry.
        /// </summary>
        [Fact]
        public void TestFailSendWithRetry()
        {
            Assert.Throws(typeof(AggregateException), () =>
             {
                 IotApi api = new IotApi();
                 api
                 .RegisterRetryModule(2, TimeSpan.FromSeconds(1))
                 .RegisterPersistModule(new Dictionary<string, object>())
                 .RegisterModule(getDefaultModule(true));

                 api.Open(new System.Collections.Generic.Dictionary<string, object>());

                 api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                     (msgs) =>
                     {
                         Assert.True(msgs.Count == 1);
                     },
                     (msgs, err) =>
                     {
                         throw err;
                     },
                     null).Wait();
             });
        }

        [Fact]
        public void TestReceiveWithCallback()
        {
            IotApi api = new IotApi()
              .RegisterPersistModule(new Dictionary<string, object>())
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

        [Fact]
        public void TestReceive()
        {
            IotApi api = new IotApi()
              .RegisterPersistModule(new Dictionary<string, object>())
              .RegisterModule(getDefaultModule(true));

            api.Open(new System.Collections.Generic.Dictionary<string, object>());

            var msg = api.ReceiveAsync().Result;
        }


        private void testMethod(int a)
        {

        }

        public void go(dynamic a, dynamic b)
        {

        }
    }
}
