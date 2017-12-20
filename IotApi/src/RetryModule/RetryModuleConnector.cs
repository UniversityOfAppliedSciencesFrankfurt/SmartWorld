using Iot;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RetryModule
{
    public class RetryModuleConnector : ISendModule
    {
        private ISendModule m_NextModule;
        private string m_SelectedCase;
        private int m_DelayInMlliseconds;
        private int m_RetryCount;

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

        public RetryModuleConnector(string selectedCase, int retryCount, int delayInMlliseconds)
        {
            this.m_SelectedCase = selectedCase;
            this.m_RetryCount = retryCount;
            this.m_DelayInMlliseconds = delayInMlliseconds;
        }

        public void Open(Dictionary<string, object> args)
        {

        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null,
            Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {

            switch (m_SelectedCase.ToLower())
            {
                case "retry":
                    await Retry(sensorMessage, onSuccess, onError, args);
                    break;
                case "exponential":
                    await RetryExponentially(sensorMessage, onSuccess, onError, args);
                    break;

                case "geometric":
                    await RetryGeometrically(sensorMessage, onSuccess, onError, args);
                    break;

                default:
                    break;
            }
        }

        public async Task Retry(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            for (int a = 0; a < m_RetryCount; a++)
            {
                await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                            a = 0;
                        },
                        (err) =>
                        {
                            if (a == 0)
                                onError?.Invoke(err);
                            else
                                Thread.Sleep(TimeSpan.FromMilliseconds(m_DelayInMlliseconds));

                        }, args);

            }
        }

        public async Task RetryExponentially(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            for (int a = 0; a < m_RetryCount; a = a + 1)
            {

                try
                {
                    await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                        },
                        (err) =>
                        {
                            onError?.Invoke(err);
                        }, args);
                }
                catch
                {
                    var delayTime = System.Convert.ToInt32(Math.Pow(Convert.ToDouble(m_DelayInMlliseconds), Convert.ToDouble(a)));
                    Thread.Sleep(TimeSpan.FromMilliseconds(delayTime));
                    continue;
                }
            }
        }

        public async Task RetryGeometrically(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            for (int a = 0; a < m_RetryCount; a = a + 1)
            {

                try
                {
                    await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                        },
                        (err) =>
                        {
                            onError?.Invoke(err);
                        }, args);
                }
                catch
                {
                    var delayTime = m_DelayInMlliseconds * a;
                    Thread.Sleep(TimeSpan.FromMilliseconds(delayTime));
                    continue;
                }

            }
        }
    }
}
