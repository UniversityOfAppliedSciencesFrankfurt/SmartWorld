using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dacs7;
using Dacs7.Domain;

namespace ProfinetConnector
{
    public class ProfinetConnector : ISendModule,IReceiveModule

    {
        private Dacs7Client m_Client;
        private string m_ConnectionString;
        public IReceiveModule NextReceiveModule { get; set; }
        public ISendModule NextSendModule { get; set; }

        int length = 500;
        byte[] testData = new byte[500];
        int offset = 10; //The offset is the number of bytes from the beginning of the area
        int dbNumber = 560;

        public void Open(Dictionary<string, object> args)
        {
            if(!String.IsNullOrEmpty(m_ConnectionString))
                m_Client.Connect(m_ConnectionString);
        }

        public ProfinetConnector(string connStr)
        {
            this.m_ConnectionString = connStr;
            this.m_Client = new Dacs7Client();
        }

        public Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
           
        }

        public Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            return Task.Run(() =>
            {
                var result = m_Client.ReadAny(PlcArea.DB, offset, typeof(byte), new[] { length, dbNumber });
            });
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            return Task.Run(() =>
            {
                //Write an array of bytes to the PLC. 
                m_Client.WriteAny(PlcArea.DB, offset, testData, new[] { length, dbNumber });
            });
        }
    }
}
