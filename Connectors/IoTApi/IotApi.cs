using Daenet.Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Daenet.IoT.Services;

namespace Daenet.Iot
{
    public class IotApi : IIotApi
    {
        private IIotApi m_Connector;

        private List<IInjectableModule> m_Modules = new List<IInjectableModule>();


        public IotApi(IIotApi connector, ICollection<IInjectableModule> injectableModules)
        {
            foreach (var svc in injectableModules)
            {
                m_Modules.Add(svc);
            }

            m_Connector = connector;
        }

        private IEnumerable<T> getServices<T>()
        {
            return m_Modules.OfType<T>();
        }

        public string Name { get { return m_Connector.Name; } }

        public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        {
            return m_Connector.OnMessage(onReceiveMsg, cancelationToken, args);
        }

        public Task Open(Dictionary<string, object> args)
        {
            return m_Connector.Open(args);
        }

        public Task ReceiveAsync(Func<object, bool> onSuccess = null, Func<Exception, bool> onError = null, int timeout = 60000, Dictionary<string, object> args = null)
        {
            return m_Connector.ReceiveAsync(onSuccess, onError, timeout, args);
        }

        public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
        {
            m_Connector.RegisterAcknowledge(onAcknowledgeReceived);
        }

        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            try
            {
                foreach (var msg in sensorMessages)
                {
                    await m_Connector.SendAsync(sensorMessages, null, null, args);
                }

                onSuccess?.Invoke(sensorMessages);
            }
            catch (Exception ex)
            {
                onError?.Invoke(sensorMessages, ex);
            }
        }


        public async Task SendAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            try
            {
                foreach (var service in getServices<IInjectableModule>())
                {
                    if (await service.Send(m_Connector, sensorMessage, (msgs)=> 
                    {

                    },
                    (msgs, err) =>                    
                    {
                        onError?.Invoke(new List<object> { sensorMessage }, err);
                    },                    
                    args))
                        break;
                }

                onSuccess?.Invoke(new List<object> { sensorMessage } );
            }
            catch (Exception ex)
            {
                onError?.Invoke(new List<object> { sensorMessage }, ex);
            }
        }

    }
}
