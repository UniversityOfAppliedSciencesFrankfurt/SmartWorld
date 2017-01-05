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


        /// <summary>
        /// 
        /// </summary>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args">If null, then test will succsessfully return a message.
        /// If none null, then an error will be simulated.</param>
        /// <returns></returns>
        public async Task ReceiveAsync(Action<IList<object>> onSuccess = null,
            Action<IList<object>, Exception> onError = null,
            Dictionary<string, object> args = null)
        {
            await Task.Delay(5000);

            await Task.Run(() =>
            {
                TelemetryData sensorEvent = new TelemetryData()
                {
                    Device = "DEVICE001",
                    Temperature = DateTime.Now.Minute,
                };

                if (args == null)
                    onSuccess?.Invoke(new List<object> { sensorEvent });
                else
                    onError?.Invoke(new List<object> { sensorEvent }, new Exception("UnitTest controlled error"));
                
            });
        }

        public async Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            await Task.Delay(5000);

            return Task.Run(() =>
            {
                TelemetryData sensorEvent = new TelemetryData()
                {
                    Device = "DEVICE001",
                    Temperature = DateTime.Now.Minute,
                };

                return sensorEvent;
            });
        }

     
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
