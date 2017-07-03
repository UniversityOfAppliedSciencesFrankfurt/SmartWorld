using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using NUnit;

namespace CoAPConnector
{
    public class CoAPClientConnector : ISendModule , IReceiveModule
    {
        public readonly int MaxTaskTimeout = System.Diagnostics.Debugger.IsAttached ? -1 : 2000;

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

        private IReceiveModule m_NextReceiveModule;

        public IReceiveModule NextReceiveModule
        {
            get
            {
                return m_NextReceiveModule;
            }

            set
            {
                m_NextReceiveModule = value;
            }
        }





        ICoapEndpoint endpoint;

        public void Open(Dictionary<string, object> args)
        {
           if(args != null)
            {
                var obj = args["endPoint"];
                if (obj != null)
                {
                    endpoint = obj as ICoapEndpoint;
                }
                else
                {
                    throw new Exception("Must use ICoapEndpoint interface.");
                }
            }
        }

        public async Task SendAsync(object sensorMessage, 
                                Action<object> onSuccess = null, 
                                Action<IotApiException> onError = null, 
                                Dictionary<string, object> args = null)
        {
            try
            {
                CoapClient client = new CoapClient(endpoint);

                client.listen();
                var mgs = sensorMessage as CoapMessage;
                var result = await client.SendAsync(mgs);

                if (result != 0)
                    onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(new IotApiException(ex.Message));
            }
        }

        public Task SendAsync(IList<object> sensorMessages, 
                                Action<IList<object>> onSuccess = null, 
                                Action<IList<IotApiException>> onError = null, 
                                Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task ReceiveAsync (Action<IList<object>> onSuccess,
                                       Action<IList<object>, Exception> onError,
                                       Dictionary<string, object> args)
        {
            CoapClient client = new CoapClient(endpoint);

            var sendTask = client.GetAsync("coap://example.com/.well-known/core");
            sendTask.Wait(MaxTaskTimeout);

            if (!sendTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("sendTask took too long to complete");

            client.listen();

            var responseTask = client.GetResponseAsync(sendTask.Result);
            responseTask.Wait(MaxTaskTimeout);

            if (!responseTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("responseTask took too long to complete");
        }

        public async Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

    }
}
