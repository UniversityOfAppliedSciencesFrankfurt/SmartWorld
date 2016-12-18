using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Iot
{
    public static class PersistExtension
    {
        public static IotApi RegisterPersistModule(this IotApi api, Dictionary<string, object> args = null)
        {
            PersistModule module = new PersistModule();
            api.RegisterModule(module);

            return api;
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class PersistModule : ISendModule
    {
        private ISendModule m_NextModule;

        public ISendModule NextSendModule
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


        /// <summary>
        /// Nothing to do.
        /// </summary>
        /// <param name="args"></param>
        public void Open(Dictionary<string, object> args)
        {

        }

        /// <summary>
        /// Simulate sending of a signle message.
        /// </summary>
        /// <param name="sensorMessages"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            foreach (var msg in sensorMessages)
            {
                await SendAsync(msg, onSuccess, onError, args);
            }
        }


        /// <summary>
        /// Simulate sending of a signle message.
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task SendAsync(object sensorMessage,
            Action<IList<object>> onSuccess = null,
            Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            await NextSendModule.SendAsync(sensorMessage, (msgs) =>
            {
                onSuccess?.Invoke(new List<object> { sensorMessage });
            },
            (msgs, err) =>
            {
                onError?.Invoke(new List<object> { sensorMessage }, err);
            },
            args);
        }
    }
}
