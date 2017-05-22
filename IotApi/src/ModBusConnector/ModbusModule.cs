using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;
using System.Net.Sockets;
using System.Net;

namespace ModBusConnector
{
    public class ModbusModule : ISendModule, IReceiveModule
    {
        private static IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });
        private static int port = 502;
        private static byte slaveID = 1;

        public static ModbusTCP Modbus = new ModbusTCP(address, slaveID, port);

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
            if (address== null || port != 502)
                throw new ArgumentException("Not valid");
        }

        public Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }


        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            ModbusTCP mb_tcp = new ModbusTCP(address, slaveID, port);

            await Task.Run(() =>
            {

                try
                {
                    onSuccess?.Invoke(new List<object>())
                }
                catch
                {
                    onError?.Invoke(new List<object>)
                }
            });
            
            throw new NotImplementedException();
            
            
        }

        
    }
}
