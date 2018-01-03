using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Iot;

namespace ProfinetConnectorUnitTest
{
    class DummyConnectorThree : IReceiveModule, ISendModule
    {
        public IReceiveModule NextReceiveModule { get; set; }
        public ISendModule NextSendModule { get; set; }

        public void Open(Dictionary<string, object> args)
        {
        }

        public Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            return await Task.Run<object>(() =>
            {
                return "This is receive function";
            });
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
