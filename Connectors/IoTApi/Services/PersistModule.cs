using Daenet.Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.IoT.Services
{
    public static class PersistExtension
    {
        public static IotApi RegisterPersist(this IotApi api, Dictionary<string, object> args = null)
        {
            PersistModule module = new Services.PersistModule();
            api.RegisterModule(module);

            return api;
        }

    }
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

        public void Open(Dictionary<string, object> args)
        {
           
        }

        public Task ReceiveAsync(object sensorMessage, Action<IList<object>> onSuccess = null, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }



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
