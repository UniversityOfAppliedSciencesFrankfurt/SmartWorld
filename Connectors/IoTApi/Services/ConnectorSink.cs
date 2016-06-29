using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daenet.Iot;
using Daenet.IoT.Services;

namespace Daenet.IoT.Services
{
    public class ConnectorSink : IInjectableModule, IBeforeReceive
    {
        private IIotApi m_Connector;

        public async Task Open(IIotApi connector, Dictionary<string, object> args = null)
        {
            if (m_Connector == null)
                throw new IotApiException("Connector must be specified!");

            await connector.Open(args);

            m_Connector = connector;
        }

        public bool BeforeReceive(IIotApi connector, object sensorMessage, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }


        public async Task<bool> Send(IIotApi connector, object sensorMessage, Action<IList<object>> onSuccess = null,
           Action<IList<object>, Exception> onError = null,
           Dictionary<string, object> args = null)
        {
            return await Task.Run(() =>
            {
                m_Connector.SendAsync(sensorMessage, onSuccess, onError, args).Wait();

                return true;
            });
        }

      
    }
}
