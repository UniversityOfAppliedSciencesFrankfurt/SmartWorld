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
    public class IotApi 
    {
        private bool m_IsOpenCalled = false;

        private IIotApi m_Connector;

        private List<IInjectableModule> m_Modules = new List<IInjectableModule>();


        public IotApi(ICollection<IInjectableModule> injectableModules = null)
        {
            if (injectableModules == null)
            {
                injectableModules = new List<IInjectableModule>();
               // injectableModules.Add(new IoT.Services.ConnectorSink());
            }

            foreach (var svc in injectableModules)
            {
                m_Modules.Add(svc);
            }
        }

        public IotApi RegisterModule(IInjectableModule module)
        {
            m_Modules.Add(module);
            return this;
        }

        private IEnumerable<T> getServices<T>()
        {
            return m_Modules.OfType<T>();
        }

        public string Name { get { return m_Connector.Name; } }

        public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            return m_Connector.OnMessage(onReceiveMsg, cancelationToken, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">COntains list of all configuration parameters for all injectable modules.</param>
        /// <returns></returns>
        public void Open(Dictionary<string, object> args)
        {
            int cnt = 1;
            foreach (var module in m_Modules)
            {
                module.NextModule = m_Modules.Count > cnt ? m_Modules[cnt] : null;
                cnt++;
            }

            m_IsOpenCalled = true;
        }

        public Task ReceiveAsync(Func<object, bool> onSuccess = null, Func<Exception, bool> onError = null, int timeout = 60000, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            return m_Connector.ReceiveAsync(onSuccess, onError, timeout, args);
        }

        public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            m_Connector.RegisterAcknowledge(onAcknowledgeReceived);
        }

        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            try
            {
                foreach (var msg in sensorMessages)
                {
                    await this.SendAsync(sensorMessages, (msgs)=> {

                    },
                    (msgs, err) =>
                    {
                        onError?.Invoke(new List<object> { msg }, err);
                        return;
                    },
                    args);
                }

                onSuccess?.Invoke(sensorMessages);
            }
            catch (Exception ex)
            {
                onError?.Invoke(sensorMessages, ex);
            }
        }


        public async Task SendAsync(object sensorMessage, 
            Action<IList<object>> onSuccess = null, Action<IList<object>, 
                Exception> onError = null, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            try
            {
                var module = getServices<IInjectableModule>().FirstOrDefault();
                if (module != null)
                {
                   // while(module != null)
                   // {
                        await module.SendAsync(sensorMessage,
                        (msgs) =>
                        {

                        },
                        (msgs, err) =>
                        {
                            onError?.Invoke(new List<object> { sensorMessage }, err);
                        },
                        args);
                   // };

                }

                /*
                foreach (var module in getServices<IInjectableModule>())
                {
                    if (await module.Send(sensorMessage, (msgs)=> 
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
                */
            }
            catch (Exception ex)
            {
                onError?.Invoke(new List<object> { sensorMessage }, ex);
            }
        }
    }
}
