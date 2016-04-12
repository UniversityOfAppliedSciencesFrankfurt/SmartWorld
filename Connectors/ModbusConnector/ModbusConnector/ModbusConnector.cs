using Daenet.Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModbusConnector
{
    public class ModbusConnector : IIotApi
    {
        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public void AcknowledgeReceivedMessage(string msgId, Exception error = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public void OnMessage(Action<object> onReceiveMsg, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task Open(Dictionary<string, object> args)
        {
            throw new NotImplementedException();
        }

        public object Receive(Dictionary<string, object> args = null)
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
