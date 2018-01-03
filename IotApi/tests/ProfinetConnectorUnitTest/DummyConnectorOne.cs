using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Iot;

namespace ProfinetConnectorUnitTest
{
    class DummyConnectorOne : ISendModule
    {
        public ISendModule NextSendModule { get; set; }

        public void Open(Dictionary<string, object> args)
        {
        }

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
             await Task.Run(() =>
            {

            });
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}
