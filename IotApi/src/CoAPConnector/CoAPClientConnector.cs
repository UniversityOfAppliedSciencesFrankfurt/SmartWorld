using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using NUnit;
using Moq;


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

        private CoapClient client;

        public void Open(Dictionary<string, object> args)
        {
            ICoapEndpoint endpoint;
            if (args != null)
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
                client = new CoapClient(endpoint);
            }
        }

        // Asynchronous sending 1
        public async Task SendAsync(object sensorMessage, 
                                Action<object> onSuccess = null, 
                                Action<IotApiException> onError = null, 
                                Dictionary<string, object> args = null)
        {
            client.Listen();
            try
            {
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
        // Asynchronous sending 2
        public Task SendAsync(IList<object> sensorMessages, 
                                Action<IList<object>> onSuccess = null, 
                                Action<IList<IotApiException>> onError = null, 
                                Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
        // Asynchronous receiving 1
        public async Task ReceiveAsync (Action<IList<object>> onSuccess,
                                       Action<IList<object>, Exception> onError,
                                       Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }
        // Asynchronous receiving 2
        public async Task<object> ReceiveAsync(Dictionary<string, object> args)
        {
            string uri = args["URI"].ToString();
            var sendTask = client.GetAsync(uri);
            sendTask.Wait(MaxTaskTimeout);

            if (!sendTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("sendTask took too long to complete");

            client.Listen();

            var responseTask = client.GetResponseAsync(sendTask.Result);
            responseTask.Wait(MaxTaskTimeout);

            if (!responseTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("responseTask took too long to complete");

            return await responseTask;
        }

        public bool listenertest(Mock<ICoapEndpoint> mock)
        { 
            var clientOnMessageReceivedEventCalled = false;
            var task = new TaskCompletionSource<bool>();
            client.OnMessageReceived += (s, e) =>
            {
                clientOnMessageReceivedEventCalled = true;
                task.SetResult(true);
            };

            client.Listen();

            task.Task.Wait(MaxTaskTimeout);
            return clientOnMessageReceivedEventCalled;
        }
    }
}
