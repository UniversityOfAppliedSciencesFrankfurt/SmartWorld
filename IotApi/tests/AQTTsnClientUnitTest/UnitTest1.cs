//using Iot;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//namespace AQTTsnClientUnitTest
//{
//    [TestClass]
//    public class UnitTest1
//    {
//        static int subID = 1;
//        static int pubId = 1;
//        static int regId = 1;

//        [TestMethod]
//        public void ConnectTest()
//        {
//            IotApi api = new IotApi();
//            api.RegisterModule(new MQTTSnConnector());

//            Dictionary<string, object> agr = new Dictionary<string, object>();
//            agr.Add("ip", "127.0.0.1");

//            agr.Add("port", 100);

//            api.Open(agr);

//            MessageInterface.ConnectWrk connect = new MessageInterface.ConnectWrk();
//            connect.connect.clientId = ASCIIEncoding.ASCII.GetBytes("0100");
//            connect.connect.flags = Flag.cleanSession;

//            api.SendAsync(connect, (succ) =>
//            {
//                // var k =byte.Parse(r.ToString());
//                var k = succ;

//            }, (error) =>
//            {

//            });
//        }

//        [Fact]
//        public void RegisterTest()
//        {
//            IotApi api = new IotApi();
//            api.RegisterModule(new MQTTSnConnector());
//            Dictionary<string, object> agr = new Dictionary<string, object>();
//            agr.Add("ip", "127.0.0.1");

//            agr.Add("port", 100);

//            api.Open(agr);

//            byte[] topicId = ASCIIEncoding.ASCII.GetBytes("21".PadLeft(2, '0'));
//            string topicName = "21jk";

//            MessageInterface.RegisterWrk register = new MessageInterface.RegisterWrk();
//            register.register.topicId = topicId;
//            register.register.topicName = ASCIIEncoding.ASCII.GetBytes(topicName);
//            register.register.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(regId).PadLeft(2, '0'));
//            register.register.length = Convert.ToByte(6 + topicName.Length);

//            api.SendAsync(register, (succ) =>
//            {
//                var c = succ;
//            }, (error) =>
//            {

//            });

//        }



//        [Fact]
//        public void SubscribeTest()
//        {
//            IotApi api = new IotApi();
//            api.RegisterModule(new MQTTSnConnector());

//            Dictionary<string, object> agr = new Dictionary<string, object>();
//            agr.Add("ip", "127.0.0.1");

//            agr.Add("port", 100);

//            api.Open(agr);

//            MessageInterface.SubscribeWrk subscribe = new MessageInterface.SubscribeWrk();
//            subscribe.subscribe.topicId = ASCIIEncoding.ASCII.GetBytes("66".PadLeft(2, '0'));
//            subscribe.subscribe.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(subID).PadLeft(2, '0'));

//            api.SendAsync(subscribe, (succ) =>
//            {
//                var r = succ;

//            }, (error) =>
//            {

//            });
//        }

//        [Fact]
//        public void PublishTest()
//        {
//            IotApi api = new IotApi();
//            api.RegisterModule(new MQTTSnConnector());

//            Dictionary<string, object> agr = new Dictionary<string, object>();
//            agr.Add("ip", "127.0.0.1");

//            agr.Add("port", 100);

//            api.Open(agr);


//            MessageInterface.PublishWrk publish = new MessageInterface.PublishWrk();
//            publish.publish.topicId = ASCIIEncoding.ASCII.GetBytes("33".PadLeft(2, '0')); ;
//            publish.publish.data = ASCIIEncoding.ASCII.GetBytes("lights off");
//            publish.publish.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(pubId).PadLeft(2, '0'));
//            publish.publish.length = Convert.ToByte(7 + "Lights off".Length);

//            api.SendAsync(publish, (succ) =>
//            {
//                var suc = succ;

//            }, (error) =>
//            {

//            });
//        }

//        private IotApi getApi()
//        {
//            var api = new IotApi()
//                .UseMQTTSnClient
            
//        }

//    }
//}
