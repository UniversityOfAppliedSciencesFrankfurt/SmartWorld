using Iot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace IotApiTests
{
    /// <summary>
    /// Demonstrates how to implement a sample connector.
    /// </summary>
    public class SampleProtocol : ISendModule, IReceiveModule
    {
        public string Name
        {
            get
            {
                return "SampleConnector";
            }
        }

        public ISendModule NextSendModule { get; set; }

        public IReceiveModule NextReceiveModule { get; set; }

        private bool m_SimulateErrorOnSend;

        public SampleProtocol(bool simulateErrorOnSend = false)
        {
            m_SimulateErrorOnSend = simulateErrorOnSend;
        }

        //public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        //{
        //    throw new NotImplementedException();
        //}

        public void Open(Dictionary<string, object> args = null)
        {

        }

        public Task<object> ReceiveAsync(Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            Task.Delay(5000);

            return Task<object>.Run(() =>
            {
                TelemetryData sensorEvent = new TelemetryData()
                {
                    Device = "DEVICE001",
                    Temperature = DateTime.Now.Minute,
                };

                return (object)sensorEvent;
            });
        
        }

        //public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            foreach (var msg in sensorMessages)
            {
                await this.SendAsync(msg, (msgs) =>
                {

                },
                (msgs, err) =>
                {
                    onError?.Invoke(new List<object> { msg }, err);
                },
                args);
            }

            onSuccess?.Invoke(sensorMessages);
        }


        public async Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            await Task.Run(() =>
            {
                if (m_SimulateErrorOnSend)
                {
                    onError?.Invoke(new List<object> { sensorMessage }, new Exception("Simulated error."));
                }
                else
                {
                    onSuccess?.Invoke(new List<object> { sensorMessage });
                }
            });
        }
    }
}
