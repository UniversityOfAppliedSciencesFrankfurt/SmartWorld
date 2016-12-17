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

        private List<ISendModule> m_SendModules = new List<ISendModule>();

        private List<IReceiveModule> m_ReceiveModules = new List<IReceiveModule>();

        public IotApi(ICollection<IInjectableModule> injectableModules = null)
        {
            m_SendModules = new List<ISendModule>();
            m_ReceiveModules = new List<IReceiveModule>();

            if (injectableModules != null)
            {
                foreach (var svc in injectableModules)
                {
                    if (svc is ISendModule)
                        m_SendModules.Add(svc as ISendModule);

                    if (svc is IReceiveModule)
                        m_ReceiveModules.Add(svc as IReceiveModule);
                }
            }
        }

        public IotApi RegisterModule(IInjectableModule module)
        {
            if (module is ISendModule)
                m_SendModules.Add(module as ISendModule);

            if (module is IReceiveModule)
                m_ReceiveModules.Add(module as IReceiveModule);

            return this;
        }


        private IEnumerable<T> getServices<T>()
        {
            return m_SendModules.OfType<T>();
        }


        //public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        //{
        //    if (m_IsOpenCalled == false)
        //        throw new IotApiException("Method Open must be called first.");

        //    return m_Connector.OnMessage(onReceiveMsg, cancelationToken, args);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args">COntains list of all configuration parameters for all injectable modules.</param>
        /// <returns></returns>
        public void Open(Dictionary<string, object> args = null)
        {
            int cnt = 1;
            foreach (var module in m_SendModules)
            {
                module.NextSendModule = m_SendModules.Count > cnt ? m_SendModules[cnt] : null;
                module.Open(args);
                cnt++;
            }

            cnt = 1;
            foreach (var module in m_ReceiveModules)
            {
                module.NextReceiveModule = m_ReceiveModules.Count > cnt ? m_ReceiveModules[cnt] : null;
                module.Open(args);
                cnt++;
            }

            m_IsOpenCalled = true;
        }

        public async Task<object> ReceiveAsync(Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            try
            {
                var module = getServices<IReceiveModule>().FirstOrDefault();
                if (module != null)
                {
                    var msg = await module.ReceiveAsync(onSuccess, onError, args);
                    return msg;
                }
                else
                    throw new InvalidOperationException("No registered IReceiveModule found.");
            }
            catch (Exception ex)
            {
                onError?.Invoke(null, ex);
                throw ex;
            }
        }

        //public void RegisterAcknowledge(Action<string, Exception> onAcknowledgeReceived)
        //{
        //    if (m_IsOpenCalled == false)
        //        throw new IotApiException("Method Open must be called first.");

        //    m_Connector.RegisterAcknowledge(onAcknowledgeReceived);
        //}

        public async Task SendAsync(IList<object> sensorMessages, 
            Action<IList<object>> onSuccess, 
            Action<IList<object>, Exception> onError, 
            Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            try
            {
                foreach (var msg in sensorMessages)
                {
                    await this.SendAsync(sensorMessages, (msgs) =>
                    {

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
         Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");
            try
            {
                var module = getServices<ISendModule>().FirstOrDefault();
                if (module != null)
                {
                    await module.SendAsync(sensorMessage,
                    (msgs) =>
                    {

                    },
                    (msgs, err) =>
                    {
                        throw new IotApiException("Failed to send th emessage.", err);
                    },
                    args);
                }
            }
            catch (Exception ex)
            {
                throw new IotApiException("Failed to send th emessage.", ex);
            }
        }



        public async Task SendAsync(object sensorMessage,
            Action<IList<object>> onSuccess, Action<IList<object>,
                Exception> onError, Dictionary<string, object> args = null)
        {
            if (m_IsOpenCalled == false)
                throw new IotApiException("Method Open must be called first.");

            try
            {
                var module = getServices<ISendModule>().FirstOrDefault();
                if (module != null)
                {
                    await module.SendAsync(sensorMessage,
                    (msgs) =>
                    {

                    },
                    (msgs, err) =>
                    {
                        onError?.Invoke(new List<object> { sensorMessage }, err);
                    },
                    args);
                }
            }
            catch (Exception ex)
            {
                onError?.Invoke(new List<object> { sensorMessage }, ex);
            }
        }
    }
}
