using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot
{
    /// <summary>
    /// Unified IoT Application Provider Interface.
    /// </summary>
    public class IotApi
    {
        private bool m_IsOpenCalled = false;

        private List<ISendModule> m_SendModules = new List<ISendModule>();

        private List<IReceiveModule> m_ReceiveModules = new List<IReceiveModule>();

        private List<IReceiveModule> m_AckoledgeModules = new List<IReceiveModule>();

        #region Constructors and Initialization


        /// <summary>
        /// Creates the instance of IoT API.
        /// </summary>
        /// <param name="injectableModules">List of modules, which will be executed in specified order.</param>
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


        /// <summary>
        /// Use this method to register any additional module, which 
        /// will be added to execution pipeline of the current instance of<see cref="IoTApi"/> .
        /// </summary>
        /// <param name="module">Instance of injectable module.</param>
        /// <returns>Current instance of api.</returns>
        public IotApi RegisterModule(IInjectableModule module)
        {
            if (module is ISendModule)
                m_SendModules.Add(module as ISendModule);

            if (module is IReceiveModule)
                m_ReceiveModules.Add(module as IReceiveModule);

            return this;
        }
        #endregion


        //public Task OnMessage(Func<object, bool> onReceiveMsg, CancellationToken cancelationToken, Dictionary<string, object> args = null)
        //{
        //    if (m_IsOpenCalled == false)
        //        throw new IotApiException("Method Open must be called first.");

        //    return m_Connector.OnMessage(onReceiveMsg, cancelationToken, args);
        //}

        #region Public Methods
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



        /// <summary>
        /// Sends the message to remote endpoint.
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <exception cref="IotApiException">Thrown if any exception has been thrown internally.</exception>
        /// <returns>Tesk</returns>
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


        /// <summary>
        /// Sends the batch of messages to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessages">List of messages to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
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


        /// <summary>
        /// Sends the message to remote endpoint by using of JAVA Script API style.
        /// </summary>
        /// <param name="sensorMessage">The message to be sent.</param>
        /// <param name="onSuccess">Callback function invoked after th emessage has been successfully
        /// sent to endpoint.</param>
        /// <param name="onError">Callback error function invoked if the message transfer ha failed.</param>
        /// <param name="args">Any protocol required parameters.</param>
        /// <returns>Task</returns>
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
                        onSuccess?.Invoke(msgs);
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
        #endregion

        #region Private Methods

        private IEnumerable<T> getServices<T>()
        {
            return m_SendModules.OfType<T>();
        }
        #endregion
    }
}
