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
    /// <summary> 
    /// Description
    /// </summary>
    /// Creating the connector between CoAP protocol and IotApi module
    /// Inherit from interface ISendModule and IReceiveModule
    /// Implemented method: Asynchronous sending 1, Asynchronous receiving 2
    public class CoAPclientConnector : ISendModule , IReceiveModule
    {
        #region member variables
        public readonly int m_MaxTaskTimeout = System.Diagnostics.Debugger.IsAttached ? -1 : 2000;
        private ISendModule m_NextSendModule;
        private IReceiveModule m_NextReceiveModule;
        private Coapclient m_client;
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
        #endregion

        /// <summary>
        /// Open an endpoint in IotApi module 
        /// </summary>
        /// <param name="args">read in argurment "endpoints"</param>
        /// <exception cref="Exception(string message)"></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <remarks>Required by public methods</remarks>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        public void Open(Dictionary<string, object> args)
        {
            ICoapEndpoint endPoint;
            if (args != null)
            {
                var obj = args["endPoint"];
                if (obj != null)
                {
                    endPoint = obj as ICoapEndpoint;
                }
                else
                {
                    throw new Exception("Must use ICoapEndpoint interface.");
                }
                m_client = new Coapclient(endPoint);
            }
        }

        /// <summary>
        /// Asynchronous sending 1 
        /// </summary>
        /// <param name="sensorMessage">Message to send</param>
        /// <exception cref="onError?">Error exception while sending asynchronously</exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <remarks>Required by public methods</remarks>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        public async Task SendAsync(object sensorMessage, 
                                Action<object> onSuccess = null, 
                                Action<IotApiException> onError = null, 
                                Dictionary<string, object> args = null)
        {
            m_client.Listen();
            try
            {
                var mgs = sensorMessage as CoapMessage;
                var result = await m_client.SendAsync(mgs);

                if (result != 0)
                    onSuccess?.Invoke(result);
            }
            catch (Exception ex)
            {
                onError?.Invoke(new IotApiException(ex.Message));
            }
        }

        /// <summary>
        /// Asynchronous sending 2 
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>Not implemented</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <remarks>Required by public methods</remarks>
        /// <permission cref=""></permission>
        /// <exception cref=""></exception>
        public Task SendAsync(IList<object> sensorMessages, 
                                Action<IList<object>> onSuccess = null, 
                                Action<IList<IotApiException>> onError = null, 
                                Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronous receiving 1 
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>Not implemented</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <remarks>Required by public methods</remarks>
        /// <permission cref=""></permission>
        /// <exception cref=""></exception>
        public async Task ReceiveAsync (Action<IList<object>> onSuccess,
                                       Action<IList<object>, Exception> onError,
                                       Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Asynchronous receiving 2 
        /// </summary>
        /// <param name="args">read in destination Uri"</param>
        /// <exception cref="AssertionException">check for sending and receiving time</exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <remarks>Required by public methods</remarks>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>        
        public async Task<object> ReceiveAsync(Dictionary<string, object> args)
        {
            string uri = args["URI"].ToString();
            var sendTask = m_client.GetAsync(uri);
            sendTask.Wait(m_MaxTaskTimeout);

            if (!sendTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("sendTask took too long to complete");

            m_client.Listen();

            var responseTask = m_client.GetResponseAsync(sendTask.Result);
            responseTask.Wait(m_MaxTaskTimeout);

            if (!responseTask.IsCompleted)
                throw new NUnit.Framework.AssertionException("responseTask took too long to complete");

            return await responseTask;
        }

        //public bool listenertest(Mock<ICoapEndpoint> mock)
        //{ 
        //    var clientOnMessageReceivedEventCalled = false;
        //    var task = new TaskCompletionSource<bool>();
        //    m_client.OnMessageReceived += (s, e) =>
        //    {
        //        clientOnMessageReceivedEventCalled = true;
        //        task.SetResult(true);
        //    };

        //    m_client.Listen();

        //    task.Task.Wait(m_MaxTaskTimeout);
        //    return clientOnMessageReceivedEventCalled;
        //}
    }
}
