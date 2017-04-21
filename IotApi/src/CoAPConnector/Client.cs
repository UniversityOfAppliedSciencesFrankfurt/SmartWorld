using CoAPConnetor;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace CoAPConnector
{
    /// <description>
    /// Create class receive message and endpoint field
    /// 2 fields behave as event handler
    /// </description>
    public class CoapMessageReceivedEventArgs : EventArgs
    {
        public CoapMessage Message { get; set; }
        public ICoapEndpoint Endpoint { get; set; }
    }

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

    /// <description>
    /// Create CoAP client
    /// initilize functionality of a CoAP endpoints
    /// </description>
    public class CoapClient : IDisposable
    {
        private ICoapEndpoint _transport;
        private ushort _messageId;

        private ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>> _messageReponses
            = new ConcurrentDictionary<int, TaskCompletionSource<CoapMessage>>();

        private CancellationTokenSource _receiveCancellationToken;

        public event EventHandler<CoapMessageReceivedEventArgs> OnMessageReceived;
        public event EventHandler<EventArgs> OnClosed;

        /// <description>
        /// Method to initialize transportation protocol and message ID
        /// </description>
        public CoapClient(ICoapEndpoint transport)
        {
            _transport = transport;
            _messageId = (ushort)(new Random().Next() & 0xFFFFu);
        }

        public bool IsListening
        { get => _receiveCancellationToken != null && !_receiveCancellationToken.IsCancellationRequested; }

        /// <description>
        /// Method handle when receiving CoAP incoming message
        /// </description>
        public void Listen()
        {
            if (IsListening)
                return;
            _receiveCancellationToken = new CancellationTokenSource();
            Task.Factory.StartNew(() =>
            {
                var token = _receiveCancellationToken.Token;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        var payload = _transport.ReceiveAsync();
                        payload.Wait(token);
                        if (!payload.IsCompleted || payload.Result == null)
                            continue;

                        var message = new CoapMessage(_transport.IsMulticast);
                        try
                        {
                            message.Deserialise(payload.Result.Payload);
                        }
                        catch (CoapMessageFormatException)
                        {
                            if (message.Type == CoapMessageType.Confirmable && !_transport.IsMulticast)
                            {
                                Task.Run(() => SendAsync(new CoapMessage
                                {
                                    Id = message.Id,
                                    Type = CoapMessageType.Reset
                                }, payload.Result.Endpoint));
                            }
                            continue;
                        }

                        if (_messageReponses.ContainsKey(message.Id))
                            _messageReponses[message.Id].TrySetResult(message);

                        OnMessageReceived?.Invoke(this, new CoapMessageReceivedEventArgs
                        {
                            Message = message,
                            Endpoint = payload.Result.Endpoint
                        });
                    }
                    catch (CoapEndpointException)
                    {
                        _receiveCancellationToken.Cancel();
                    }
                }
                OnClosed?.Invoke(this, new EventArgs());
            });
        }

        /// <description>
        /// Method to cancel message with wrong token
        /// </description>
        public void Dispose()
        {
            _receiveCancellationToken?.Cancel();
        }

        /// <description>
        /// Asynchronous method receiving 
        /// and waiting for message response code
        /// </description>
        public async Task<CoapMessage> GetResponseAsync(int messageId)
        {
            TaskCompletionSource<CoapMessage> responseTask = null;
            if (!_messageReponses.TryGetValue(messageId, out responseTask))
                throw new ArgumentOutOfRangeException("Message.Id is not pending response");
            await responseTask.Task;

            // ToDo: if wait timed out, retry sending message with back-off delay
            _messageReponses.TryRemove(messageId, out responseTask);

            return responseTask.Task.Result;
        }

        /// <description>
        /// Method to proceed sending back response 
        /// </description>
        public async Task<int> SendAsync(CoapMessage message, ICoapEndpoint endpoint = null)
        {
            if (message.Id == 0)
                message.Id = _messageId++;

            if (message.Type == CoapMessageType.Confirmable)
                _messageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());
            await _transport.SendAsync(new CoapPayload { Payload = message.Serialise(), MessageId = message.Id, Endpoint = endpoint });
            return message.Id;
        }

        /// <description>
        /// Method to build response message 
        /// </description>
        public async Task<int> GetAsync(string uri, ICoapEndpoint endpoint = null)
        {
            var message = new CoapMessage
            {
                Id = _messageId++,
                Code = CoapMessageCode.Get,
                Type = CoapMessageType.Confirmable
            };
            message.FromUri(uri);

            _messageReponses.TryAdd(message.Id, new TaskCompletionSource<CoapMessage>());

            return await SendAsync(message, endpoint);
        }
    }
}
