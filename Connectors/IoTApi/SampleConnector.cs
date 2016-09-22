using Daenet.Iot;
using Daenet.IoT.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Daenet.IoTApi
{
    /// <summary>
    /// Demonstrates how to implement a sample connector.
    /// </summary>
    public class SampleConnector : IIotApi
    {
        public string Name
        {
            get
            {
                return "SampleConnector";
            }
        }

        public IInjectableModule NextModule
        {
            get
            {
                return null;
            }

            set
            {
                
            }
        }

        private bool m_SimulateErrorOnSend;

        public SampleConnector(bool simulateErrorOnSend = false)
        {
            m_SimulateErrorOnSend = simulateErrorOnSend;
        }

        public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task Open(Dictionary<string, object> args)
        {
            return Task.Run(() => { });
        }

        public Task ReceiveAsync(Func<object, bool> onSuccess = null, Func<Exception, bool> onError = null, int timeout = 60000, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
        {
            throw new NotImplementedException();
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
                args) ;
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
