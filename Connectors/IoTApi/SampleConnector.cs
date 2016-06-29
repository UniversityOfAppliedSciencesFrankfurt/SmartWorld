using Daenet.Iot;
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
    /// Demonstrates how to implement a sample connector to a ficive device.
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

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
