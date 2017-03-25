using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;

namespace OpcUAConnector
{
    public class Class1 
    {
        public Class1()
        {
        }

        public IReceiveModule NextReceiveModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ISendModule NextSendModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void Open(Dictionary<string, object> args)
        {
            throw new NotImplementedException();
            //TODO: 
                                                 //Connection part with client and server part 
                                                 // namespace Opc.Ua.Server { };

        }

        public Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
            //TODO: You have client now so receive part should be here
        }

        public Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            //TODO: You have client now so send part should be here
            throw new NotImplementedException();
        }
    }
}
