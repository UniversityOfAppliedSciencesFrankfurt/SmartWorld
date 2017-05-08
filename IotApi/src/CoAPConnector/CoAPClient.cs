using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CoAPConnector
{
    public class CoAPClient : ISendModule
    {
        private ISendModule m_NextSendModule;

        public ISendModule NextSendModule
        {
            get
            {
                return m_NextSendModule;
            }

            set
            {
                m_NextSendModule = value;
            }
        }
        ICoapEndpoint endpoint;
        public void Open(Dictionary<string, object> args)
        {
           if(args != null)
            {
                //TODO: check interface 
                var obj = args["endPoint"];
                Type type = obj.GetType();
                TypeInfo info = type.GetTypeInfo();
                info.GetInterface("ICoapEndpoint");
                if (nameof(obj) != "endPoint")
                {
                    throw new Exception("must use ICoapEndpoint interface.");
                }

                endpoint = obj as ICoapEndpoint;
            }
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, 
                                Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, 
                                Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            //TODO: Create client with ICoApEndpoint and send message 

            CoapClient client = new CoapClient(endpoint);

            client.listen();
            var mgs = sensorMessage as CoapMessage;
            client.SendAsync(mgs);

            throw new NotImplementedException();
        }
    }
}
