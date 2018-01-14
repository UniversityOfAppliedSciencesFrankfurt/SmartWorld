using Iot;
using MQTTSnClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MQTTSn.Common.Entity.Message;
using System.Text;
using System;

namespace AQTTsnClientUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        static int subID = 1;
        static int pubId = 1;
        static int regId = 1;

        [TestMethod]
        public void MyMethod()
        {
            
        }

        [TestMethod]
        public void ConnectTest()
        {
            var port = "100".PadLeft(4, '0');
            IotApi api = getApi();
            
            ConnectWrk connect = new ConnectWrk();
            connect.connect.clientId = ASCIIEncoding.ASCII.GetBytes(port);
            connect.connect.flags = Flag.cleanSession;

            api.SendAsync(connect, (succ) =>
             {
                 var result = succ;
             }, (obj, err) =>
             {
                 var er = err;
             }).Wait();
        }

        [TestMethod]
        public void RegisterTest()
        {
            IotApi api = getApi();

            byte[] topicId = ASCIIEncoding.ASCII.GetBytes("21".PadLeft(2, '0'));
            string topicName = "2121jk";

            RegisterWrk register = new RegisterWrk();
            register.register.topicId = topicId;
            register.register.topicName = ASCIIEncoding.ASCII.GetBytes(topicName);
            register.register.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(regId).PadLeft(2, '0'));
            register.register.length = Convert.ToByte(6 + topicName.Length);

            api.SendAsync(register, (succ) =>
            {
                var c = succ;
                Assert.IsNotNull(c);

            }, (obj, error) =>
            {
                var er = error;
            }).Wait();

        }
        
        [TestMethod]
        public void SubscribeTest()
        {
            IotApi api = getApi();

            SubscribeWrk subscribe = new SubscribeWrk();
            subscribe.subscribe.topicId = ASCIIEncoding.ASCII.GetBytes("66".PadLeft(2, '0'));
            subscribe.subscribe.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(subID).PadLeft(2, '0'));

            api.SendAsync(subscribe, (succ) =>
            {
                var r = succ;
                Assert.IsNotNull(r);

            }, (obj, error) =>
            {
                var er = error;
            }).Wait();
        }

        [TestMethod]
        public void PublishTest()
        {
            IotApi api = getApi();

            PublishWrk publish = new PublishWrk();
            publish.publish.topicId = ASCIIEncoding.ASCII.GetBytes("33".PadLeft(2, '0')); ;
            publish.publish.data = ASCIIEncoding.ASCII.GetBytes("lights off");
            publish.publish.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(pubId).PadLeft(2, '0'));
            publish.publish.length = Convert.ToByte(7 + "Lights off".Length);

            api.SendAsync(publish, (succ) =>
            {
                var suc = succ;
                Assert.IsNotNull(suc);

            }, (obj, error) =>
            {
                var er = error;
            }).Wait();
        }

        private IotApi getApi()
        {
            var api = new IotApi()
                .UseMQTTSnClient("127.0.0.1", 100);
            api.Open();
            return api;
        }

    }
}
