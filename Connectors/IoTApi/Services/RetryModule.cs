using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daenet.Iot;

namespace Daenet.IoT.Services
{
    public class RetryModule : IInjectableModule
    {
        private IIotApi m_Connector;

        private int m_NumOfRetries = 3;

        private TimeSpan m_DelayTime = TimeSpan.FromMilliseconds(1000);


        public Task Open(IIotApi connector, Dictionary<string, object> args = null)
        {
            return Task.Run(() =>
            {
                m_Connector = connector;
                if (args.ContainsKey("NumOfRetries"))
                    m_NumOfRetries =(int) args["NumOfRetries"];

                if (args.ContainsKey("DelayTimeMs"))
                    m_DelayTime = TimeSpan.FromMilliseconds((int)args["DelayTimeMs"]);

            });
        }


        public async Task<bool> Send(IIotApi connector, object sensorMessage,
           Action<IList<object>> onSuccess = null,
           Action<IList<object>, Exception> onError = null,
           Dictionary<string, object> args = null)
        {
            int cnt = m_NumOfRetries;

            while (--cnt > 0)
            {
                await m_Connector.SendAsync(sensorMessage, (msgs) =>
                {
                    onSuccess?.Invoke(new List<object> { sensorMessage });
                    cnt = 0;
                },
                (msgs, err) =>
                {
                    onError?.Invoke(new List<object> { sensorMessage }, err);
                },
                args);

                Task.Delay(m_DelayTime).Wait();
            }

            // RetryModule is the last one! No other modules should be invoked.
            return true;
        }

     
    }
}
