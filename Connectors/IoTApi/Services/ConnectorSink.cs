using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daenet.Iot;
using Daenet.IoT.Services;

namespace Daenet.IoT.Services
{
    internal class ConnectorSink : IInjectableModule, IBeforeReceive
    {
        private IIotApi m_Connector;

        private IInjectableModule m_NextModule;

        public IInjectableModule NextModule
        {
            get
            {
                return m_NextModule;
            }

            set
            {
                m_NextModule = value;
            }
        }

        public async Task Open(IIotApi connector, IInjectableModule nextModule, Dictionary<string, object> args = null)
        {
            if (connector == null)
                throw new IotApiException("Connector must be specified!");

            await connector.Open(args);

            m_NextModule = nextModule;

            m_Connector = connector;
        }

        public bool BeforeReceive(IIotApi connector, object sensorMessage, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }


        public async Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null,
           Action<IList<object>, Exception> onError = null,
           Dictionary<string, object> args = null)
        {
            await Task.Run(() =>
            {
                m_Connector.SendAsync(sensorMessage, onSuccess, onError, args).Wait();

            });
        }

      
    }
}
