using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPConnector
{
    /// <summary> 
    /// Description
    /// </summary>
    /// Get CoAP message and CoAP endpoints
    /// Inherit from interface EventArgs
    public class CoapMessageReceivedEventArgs : EventArgs
    {
        public CoapMessage m_Message { get; set; }
        public ICoapEndpoint m_Endpoint { get; set; }
    }

    /// <summary> 
    /// Description
    /// </summary>
    /// Create Exception class for the CoAP endpoint
    /// Inherit from interface Exception
    public class CoapEndpointException : Exception
    {
        /// <summary>
        /// Base exception class
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public CoapEndpointException() : base() { }

        /// <summary>
        /// CoAP exception with input String
        /// </summary>
        /// <param name="message">input string</param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public CoapEndpointException(string message) : base(message) { }

        /// <summary>
        /// CoAP exception with input String
        /// </summary>
        /// <param name="message">input string</param>
        /// <param name="innerException">input exception</param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public CoapEndpointException(string message, Exception innerException) : base(message, innerException) { }
    }

    /// <summary> 
    /// Description
    /// </summary>
    /// Creating the connector between CoAP protocol and IotApi module
    /// Inherit from interface IDisposable
    public class Coapclient : IDisposable
    {
        private ICoapEndpoint transport;
        private ushort messageId;
        private ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>> messageReponses
            = new ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>>();
        private CancellationTokenSource receiveCancellationToken;
        public event EventHandler<CoapMessageReceivedEventArgs> OnMessageReceived;
        public event EventHandler<EventArgs> OnClosed;

        /// <summary>
        /// Create CoAP Endpoint
        /// </summary>
        /// <param name="transportEndpoint">input string as ICoAPEndpoint type</param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public Coapclient(ICoapEndpoint transportEndpoint)
        {
            transport = transportEndpoint;
            messageId = (ushort)(new Random().Next() & 0xFFFFu);
        }

        /// <summary>
        /// Get the property receiveCancellationToken
        /// </summary>
        /// <value>receiveCancellationToken</value>
        public bool IsListening
        {
            get
            {
                return receiveCancellationToken != null && !receiveCancellationToken.IsCancellationRequested;
            }
        }

        /// <summary>
        /// Create Listening point
        /// </summary>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public void Listen()
        {
            if (IsListening)
                return;

            receiveCancellationToken = new CancellationTokenSource();

            Task.Factory.StartNew(() =>
            {
                var token = receiveCancellationToken.Token;

                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var payload = transport.ReceiveAsync();
                        payload.Wait(token);
                        if (!payload.IsCompleted || payload.Result == null)
                            continue;

                        var message = new CoapMessage(transport.IsMulticast);
                        try
                        {
                            message.Deserialise(payload.Result.Payload);
                        }
                        catch (CoapMessageFormatException)
                        {
                            if (message.Type == CoapMessageType.Confirmable
                                && !transport.IsMulticast)
                            {
                                Task.Run(() => SendAsync(new CoapMessage
                                {
                                    Id = message.Id,
                                    Type = CoapMessageType.Reset
                                }, payload.Result.m_Endpoint));
                            }
                            continue;
                        }

                        if (messageReponses.ContainsKey(message.Id))
                            messageReponses[message.Id].TrySetResult(message);

                        OnMessageReceived?.Invoke(this, new CoapMessageReceivedEventArgs
                        {
                            m_Message = message,
                            m_Endpoint = payload.Result.m_Endpoint
                        });
                    }
                    catch (CoapEndpointException)
                    {
                        receiveCancellationToken.Cancel();
                    }
                }
                OnClosed?.Invoke(this, new EventArgs());
            });
        }

        /// <summary>
        /// Cancels our receiver task
        /// </summary>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        public void Dispose()
        {
            receiveCancellationToken?.Cancel();
        }

        /// <summary>
        /// Asynchronously analyse the received message
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="messageId">ID of the received message</param>
        /// <permission cref="">This method can be called by: Every User</permission>
        /// <exception cref="StatusReadDeniedException"></exception>
        public async Task<CoapMessage> GetResponseAsync(int messageId)
        {
            TaskCompletionSource<CoapMessage> responseTask = null;
            if (!messageReponses.TryGetValue(messageId, out responseTask))
                throw new ArgumentOutOfRangeException("m_Message.Id is not pending response");

            await responseTask.Task;

            // ToDo: if wait timed out, retry sending message with back-off delay
            messageReponses.TryRemove(messageId, out responseTask);

            return responseTask.Task.Result;
        }

        /// <summary>
        /// Asynchronously send out message
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="message">message to send</param>
        /// <permission cref="">This method can be called by: Every User</permission>
        /// <exception cref="StatusReadDeniedException"></exception>
        public async Task<int> SendAsync(CoapMessage message, ICoapEndpoint endpoint = null)
        {
            if (message.Id == 0)
                message.Id = messageId++;

            if (message.Type == CoapMessageType.Confirmable)
                messageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());

            await transport.SendAsync(new CoapPayload { Payload = message.Serialise(), MessageId = message.Id, m_Endpoint = endpoint });

            return message.Id;
        }

        /// <summary>
        /// Send out get request to an URI. This method is used in one of the test case
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// </summary>
        /// <param name="uri">destination URI</param>
        /// <permission cref="">This method can be called by: Every User</permission>
        /// <exception cref="StatusReadDeniedException"></exception>
        public async Task<int> GetAsync(string uri, ICoapEndpoint endpoint = null)
        {
            var message = new CoapMessage
            {
                Id = messageId++,
                Code = CoapMessageCode.Get,
                Type = CoapMessageType.Confirmable
            };
            message.FromUri(uri);

            messageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());

            return await SendAsync(message, endpoint);
        }
    }
}
