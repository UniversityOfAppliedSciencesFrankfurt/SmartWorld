using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPConnector
{
    #region Class: Setup CoAP message and endpoint properties
    /// <description>
    /// Create message and endpoint field
    /// </description>
    public class CoapMessageReceivedEventArgs : EventArgs
    {
        public CoapMessage Message { get; set; }
        public ICoapEndpoint Endpoint { get; set; }
    }
    #endregion

    #region Class: Setup CoAP exception handlers
    /// <description>
    /// Create CoAP Exception handler
    /// inherit function from System Exception
    /// </description>
    public class CoapEndpointException : Exception
    {
        public CoapEndpointException() : base() { }
        public CoapEndpointException(string message) : base(message) { }
        public CoapEndpointException(string message, Exception innerException) : base(message, innerException) { }
    }
    #endregion

    #region Class: Setup CoAP client handlers
    /// <description>
    /// Create CoAP client
    /// initilize functionality of a CoAP endpoints
    /// </description>
    public class CoapClient : IDisposable
    {
        private ICoapEndpoint Transport;
        private ushort m_MessageId;

        private ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>> MessageReponses
            = new ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>>();

        private CancellationTokenSource ReceiveCancellationToken;

        public event EventHandler<CoapMessageReceivedEventArgs> m_OnMessageReceived;
        public event EventHandler<EventArgs> m_OnClosed;

        #region Methods: Assign client parameters
        /// <summary>
        /// Method to initialize transportation protocol and message ID
        /// <seealso cref="https://tools.ietf.org/html/rfc7252"/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="messageId">Id number of the receiving message</param>
        /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
        /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager</permission>
        public CoapClient(ICoapEndpoint transport)
        {
            Transport = transport;
            m_MessageId = (ushort)(new Random().Next() & 0xFFFFu);
        }

        public CoapClient()
        {
        }

        #endregion

        public bool m_IsListening
        {
            get
            {
                return ReceiveCancellationToken != null && !ReceiveCancellationToken.IsCancellationRequested;
            }
        }

        #region Method: CoAP Listener
            /// <summary>
            /// Method handle when receiving CoAP incoming message
            /// <see also cref="https://tools.ietf.org/html/rfc7252"/>
            /// <see cref=""/>
            /// </summary>
            /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
            /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager</permission>
            /// <exception cref="CoapMessageFormatException"></exception>
            /// /// <exception cref="CoapEndpointException"></exception>
        public void listen()
        {
            if (m_IsListening)
                return;
            ReceiveCancellationToken = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                var token = ReceiveCancellationToken.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var payload = Transport.ReceiveAsync();
                        payload.Wait(token);
                        if (!payload.IsCompleted || payload.Result == null)
                            continue;

                        var message = new CoapMessage(Transport.IsMulticast);
                        try
                        {
                            message.Deserialise(payload.Result.Payload);
                        }
                        catch (CoapMessageFormatException)
                        {
                            if (message.Type == CoapMessageType.Confirmable && !Transport.IsMulticast)
                            {
                                Task.Run(() => SendAsync(new CoapMessage
                                {
                                    Id = message.Id,
                                    Type = CoapMessageType.Reset
                                }, payload.Result.Endpoint));
                            }
                            continue;
                        }

                        if (MessageReponses.ContainsKey(message.Id))
                            MessageReponses[message.Id].TrySetResult(message);

                        m_OnMessageReceived?.Invoke(this, new CoapMessageReceivedEventArgs
                        {
                            Message = message,
                            Endpoint = payload.Result.Endpoint
                        });
                    }
                    catch (CoapEndpointException)
                    {
                        ReceiveCancellationToken.Cancel();
                    }
                }
                m_OnClosed?.Invoke(this, new EventArgs());
            });
        }
        #endregion

        #region Method: CoAP disposer
        /// <summary>
        /// Method to cancel message with wrong token
        /// <see also cref="https://tools.ietf.org/html/rfc7252"/>
        /// <see cref=""/>
        /// </summary>
        /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
        /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager</permission>
        public void Dispose()
        {
            ReceiveCancellationToken?.Cancel();
        }
        #endregion

        #region Method: receiving response handler (multithread)
        /// <summary>
        /// Asynchronous method receiving and waiting for message response code
        /// <seealso cref="https://tools.ietf.org/html/rfc7252"/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="messageId">Id number of the receiving message</param>
        /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
        /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager</permission>
        public async Task<CoapMessage> GetResponseAsync(int messageId)
        {
            TaskCompletionSource<CoapMessage> responseTask = null;
            if (!MessageReponses.TryGetValue(messageId, out responseTask))
                throw new ArgumentOutOfRangeException("Message.Id is not pending response");
            await responseTask.Task;

            // ToDo: if wait timed out, retry sending message with back-off delay
            MessageReponses.TryRemove(messageId, out responseTask);

            return responseTask.Task.Result;
        }
        #endregion

        #region Method: CoAP request message
        /// <summary>
        ///  Method to proceed request message 
        /// <see also cref="https://tools.ietf.org/html/rfc7252"/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="message">field of sending message</param>
        /// <param name="endpoint">CoAP endpoint field</param>
        /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
        /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager
        public async Task<int> SendAsync(CoapMessage message, ICoapEndpoint endpoint = null)
        {
            if (message.Id == 0)
                message.Id = m_MessageId++;

            if (message.Type == CoapMessageType.Confirmable)
                MessageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());
            await Transport.SendAsync(new CoapPayload { Payload = message.Serialise(), MessageId = message.Id, Endpoint = endpoint });
            return message.Id;
        }
        #endregion

        #region Method: proceed sending back response 
        /// <summary>
        ///  Method to build response message  
        /// <see also cref="https://tools.ietf.org/html/rfc7252"/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="uri">URI address of sending CoAP client</param>
        /// <param name="endpoint">CoAP endpoint field</param>
        /// <remarks>By protected and private method is not mandatory. Use if useful</remarks>
        /// <permission cref="">This method can be called by: Administrator, Orderer, PurchasingAgent and OrderManager
        public async Task<int> GetAsync(string uri, ICoapEndpoint endpoint = null)
        {
            var message = new CoapMessage
            {
                Id = m_MessageId++,
                Code = CoapMessageCode.Get,
                Type = CoapMessageType.Confirmable
            };
            message.FromUri(uri);

            MessageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());

            return await SendAsync(message, endpoint);
        }
        #endregion
    }
    #endregion
}
