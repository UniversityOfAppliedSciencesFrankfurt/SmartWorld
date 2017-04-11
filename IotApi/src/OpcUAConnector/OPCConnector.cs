using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Iot;
using Opc.Ua.Configuration;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Sample;
using Iot;




namespace OpcUAConnector
{
    public class OPCConnector : ISendModule
    {
        public ISendModule NextSendModule
        {get; set;}

        public void Open(Dictionary<string, object> args)
        {

            string endpoint = args["endpoint"].ToString();
            try
            {

                MySampleServer server = new MySampleServer();
                server.Start();

                Task t = Client.ConsoleSampleClient(endpoint);
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exit due to Exception: {0}", e.Message);
            }

        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }
    }
}














