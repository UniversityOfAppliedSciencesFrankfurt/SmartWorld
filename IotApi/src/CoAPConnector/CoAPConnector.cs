﻿using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace CoAPConnector
{
    public class CoAPConnector : ISendModule
    {
        private ISendModule m_NextSendModule;
        private ICoapEndpoint m_Transport;

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

            CoapClient client = new CoapClient(m_Transport);
            client.listen();
            client.SendAsync();

            throw new NotImplementedException();
        }
    }
}
