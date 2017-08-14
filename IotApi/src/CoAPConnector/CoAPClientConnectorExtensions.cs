//using Iot;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Moq;
//using System.Collections.Concurrent;

//namespace CoAPConnector
//{
//    public class CoAPclientConnectorExtensions
//    {
//        private ICoapEndpoint Transport;
//        private ushort m_MessageId;
//        private ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>> MessageReponses
//            = new ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>>();

//        public IotApi getApi(Mock<ICoapEndpoint> mock)
//        {
//            Dictionary<string, object> agr = new Dictionary<string, object>();
//            IotApi api = new IotApi().RegisterModule(new CoAPConnector.CoAPclientConnector());
//            agr.Add("endPoint", mock.Object);
//            api.Open(agr);
//            return api;
//        }

//        public CoAPclientConnectorExtensions(ICoapEndpoint transport)
//        {
//            Transport = transport;
//            m_MessageId = (ushort)(new Random().Next() & 0xFFFFu);
//        }

//        public async Task<int> GetAsync(string uri, Mock<ICoapEndpoint> mock)
//        {
//            var message = new CoapMessage
//            {
//                Id = m_MessageId++,
//                Code = CoapMessageCode.Get,
//                Type = CoapMessageType.Confirmable
//            };
//            message.FromUri(uri);

//            MessageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());

//            var api = getApi(mock);
//            int result = Convert.ToInt32(await api.SendAsync(message));
//            return result;
//        }

//        public async Task<int> ReceiveAsync(int messageID, Mock<ICoapEndpoint> mock)
//        {
//            Dictionary<string, object> agr = new Dictionary<string, object>();            
//            agr.Add("sendResult", messageID);
//            var api = getApi(mock);
//            int result = Convert.ToInt32(await api.ReceiveAsync(agr));
//            return result;
//        }
//    }
//}
