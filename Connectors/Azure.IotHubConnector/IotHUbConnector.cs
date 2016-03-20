
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Diagnostics;

namespace Azure.IotHubConnector
{
    public class IotHubConnector
    {
        /// <summary>
        /// Message as serialized for IotHub and original message s sobject.
        /// </summary>
        private List<Tuple<Message, object>> m_SensorMessages = new List<Tuple<Message, object>>();

        /// <summary>
        /// Number of retries when sending message(s).
        /// </summary>
        private int m_NumOfRetries = 5;

        /// <summary>
        /// Specifies how many messages will be acumulated, before batch of
        /// messages will be sent to IoTHub.
        /// </summary>
        private int m_NumOfMessagesPerBatch = 1;

        /// <summary>
        /// Action which can be used to serialize sensor Event to Message.
        /// </summary>
        private Func<object, Message> m_SerializationFunction;


        /// <summary>
        /// Invoked when an error ocurres while sending message.
        /// </summary>
        private Action<Exception, IList<object>, int> m_OnRetryCallback;

        private DeviceClient m_DeviceClient;


        public string Name
        {
            get
            {
                return "IotHubConnector";
            }
        }

        public async Task Open(Dictionary<string, object> args)
        {
            await Task.Run(() =>
            {
                if (args.ContainsKey("NumOfMessagesPerBatch"))
                    m_NumOfMessagesPerBatch = (int)args["NumOfMessagesPerBatch"];

                if (args.ContainsKey("SerializerAction"))
                    m_SerializationFunction = jsonSerializeFunc;

                if (args.ContainsKey("OnRetryCallback"))
                    m_OnRetryCallback = onRetryCallback;

                if (args.ContainsKey("NumOfRetries"))
                    m_NumOfRetries  = (int)args["NumOfRetries"];


                string connStr = null;
                string deviceId = null;

                if (args != null)
                {
                    if (args.ContainsKey("ConnStr"))
                        connStr = (string)args["ConnStr"];
                    else
                        throw new Exception("IoTHub connection string must be provided.");

                    if (connStr.Contains("DeviceId") == false)
                    {
                        if (args.ContainsKey("DeviceId"))
                        {
                            deviceId = (string)args["DeviceId"];
                            m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr, deviceId, TransportType.Http1);
                        }
                        else
                            throw new Exception("DeviceId must be provided in argument list or in connection string.");
                    }
                    else
                        m_DeviceClient = DeviceClient.CreateFromConnectionString(connStr, TransportType.Http1);
                }
            });
        }

        #region Defaul Services

        /// <summary>
        /// Default serislization function is JSON with UTF8 encoding.
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <returns></returns>
        private static Message jsonSerializeFunc(object sensorMessage)
        {
            var messageString = JsonConvert.SerializeObject(sensorMessage);

            var message = new Message(Encoding.UTF8.GetBytes(messageString));

            return message;
        }

        private static void onRetryCallback(Exception ex, IList<object> messages, int currentRetry)
        {
            if (ex is AggregateException)
                Debug.WriteLine($"Error: {ex.InnerException.GetType().Name}, {ex.InnerException.Message}, msgId:{messages.Count}, Current retry: {currentRetry}");
            else
                Debug.WriteLine($"Error: {ex.GetType().Name}, {ex.Message}, msgId:{messages.Count}, Current retry: {currentRetry}");

        }
        #endregion


        public void OnMessage(Action<IotBridge.Message> onReceiveMsg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnSendAcknowledgeResult(Action<string, Exception> onMsgSendResult, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public IotBridge.Message Receive(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void Send(IotBridge.Message msg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(object sensorMessage,
                                    Action<IList<object>> onSuccess = null,
                                    Action<IList<object>, Exception> onError = null,
                                    Dictionary<string, object> args = null)
        {
            await SendAsync(new List<object> { sensorMessage }, onSuccess, onError, args);
        }




        public async Task SendAsync(ICollection<object> sensorMessages,
            Action<IList<object>> onSuccess = null,
            Action<IList<object>, Exception> onError = null,
            Dictionary<string, object> args = null)
        {
            try
            {
                foreach (var msg in sensorMessages)
                {
                    m_SensorMessages.Add(new Tuple<Message, object>(jsonSerializeFunc(msg), msg));
                }

                if (m_SensorMessages.Count >= m_NumOfMessagesPerBatch)
                {
                    int retries = 0;

                    while (retries < m_NumOfRetries)
                    {
                        try
                        {
                            await m_DeviceClient.SendEventBatchAsync(m_SensorMessages.Select(m=>m.Item1));

                            Debug.WriteLine($"Sent {m_SensorMessages.Count} events to cloud.");

                            try
                            {
                                onSuccess?.Invoke(new List<object>(m_SensorMessages.Select(m=>m.Item2)));
                            }
                            catch (Exception callerException)
                            {
                                // This ensures that error will be thrown without retry.
                                retries = m_NumOfRetries + 1;
                                throw callerException;
                            }

                            m_SensorMessages.Clear();

                            break;
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("Warning sending to hub!" + ex.Message);
                            retries++;

                            if (retries >= m_NumOfRetries)
                            {
                                onError?.Invoke(new List<object>(m_SensorMessages), ex);
                            }

                            m_OnRetryCallback?.Invoke(ex, m_SensorMessages.Select(m => m.Item2).ToList(), retries);

                            await Task.Delay(1000);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
