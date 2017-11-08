using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iot;





namespace OpcUAConnector
{
    public class OPCConnector : ISendModule
    {
        public ISendModule NextSendModule{get; set;}
        private string endpoint;
        public void Open(Dictionary<string, object> args)
        {

             endpoint = args["endpoint"].ToString();
           

        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            try
            {

                //MySampleServer server = new MySampleServer();
                //server.Start();

                await Client.ConsoleSampleClient(endpoint);
               
            }
            catch (Exception e)
            {
                Console.WriteLine("Exit due to Exception: {0}", e.Message);
                throw e;
            }
        }
    }
}














