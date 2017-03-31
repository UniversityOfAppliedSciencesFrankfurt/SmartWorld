using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iot;





namespace OpcUAConnector
{
    public class OPCConnector
    {
       


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
           
           
            //TODO: 
            //Connection part with client and server part 
            // namespace Opc.Ua.Server { };


        }

        public Task<string> ReceiveAsync(string endpointURL)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveAsync(Action<IList<string>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(IList<string> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

    }
}














