
using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace IotApiTests
{
    public class CoreTests
    {
        public IInjectableModule getDefaultModule(bool simulateErrorOnSend = false)
        {
            return new SampleProtocol(simulateErrorOnSend);
        }


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
       // [ExpectedException(typeof(IotApiException))]
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
        [Fact]
        public void TestSend()
        {
            IotApi api = new IotApi();

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
        }

        /// <summary>
        /// Test batched send.
        /// </summary>
        [Fact]
        public void TestBatchSend()
        {
            //todo.
        }

        /// <summary>
        /// Test sending of the message with retry.
        /// </summary>
        [Fact]
        public void TestSendWithRetry()
        {
            IotApi api = new IotApi();
            api
            .RegisterRetry(2, TimeSpan.FromSeconds(1))
            .RegisterPersist(new Dictionary<string, object>())
            .RegisterModule(getDefaultModule(false));

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
        }

        /// <summary>
        /// Test sending of the message with retry.
        /// </summary>
        [Fact]
        public void TestFailSendWithRetry()
        {
            Assert.Throws(typeof(AggregateException),() =>
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
