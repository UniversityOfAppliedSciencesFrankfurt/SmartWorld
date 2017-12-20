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
        private Case m_SelectedCase;
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

        /// <summary>
        /// Retry module constructor
        /// </summary>
        /// <param name="selectedCase">Three cases are supported, retry, exponential and geometric</param>
        /// <param name="retryCount"></param>
        /// <param name="delayInMlliseconds"></param>
        public RetryModuleConnector(Case selectedCase, int retryCount, int delayInMlliseconds)
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

            switch (m_SelectedCase)
            {
                case Case.Retry:
                    await Retry(sensorMessage, onSuccess, onError, args);
                    break;
                case Case.Exponential:
                    await RetryExponentially(sensorMessage, onSuccess, onError, args);
                    break;

                case Case.Geometric:
                    await RetryGeometrically(sensorMessage, onSuccess, onError, args);
                    break;

                default:
                    break;
            }
        }

        public async Task Retry(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            while (m_RetryCount-- > 0)
            {
                await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                            m_RetryCount = 0;
                        },
                        (err) =>
                        {
                            if (m_RetryCount == 0)
                                onError?.Invoke(err);
                            else
                                Task.Delay(m_DelayInMlliseconds).Wait();// Thread.Sleep(TimeSpan.FromMilliseconds(m_DelayInMlliseconds));

                        }, args);

            }
        }

        /// <summary>
        /// Try after every exponential number of delay, delay = 10, retryCount = 3 than delay time will be 10^1,10^2 and 10^3
        /// </summary>
        /// <param name="sensorMessage"></param>
        /// <param name="onSuccess"></param>
        /// <param name="onError"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public async Task RetryExponentially(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            int a = 1;
            while (m_RetryCount-- > 0)
            {
                await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                            m_RetryCount = 0;
                        },
                        (err) =>
                        {
                            if (m_RetryCount == 0)
                            {
                                onError?.Invoke(err);
                            }
                            else
                            {
                                var delayTime = System.Convert.ToInt32(Math.Pow(Convert.ToDouble(m_DelayInMlliseconds), Convert.ToDouble(a++)));
                                Task.Delay(TimeSpan.FromMilliseconds(delayTime)).Wait();
                            }

                        }, args);
            }
        }

        public async Task RetryGeometrically(object sensorMessage, Action<object> onSuccess = null,
            Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            int a = 1;
            while (m_RetryCount-- > 0)
            {
                await NextSendModule.SendAsync(sensorMessage,
                        (succ) =>
                        {
                            onSuccess?.Invoke(succ);
                            m_RetryCount = 0;
                        },
                        (err) =>
                        {
                            if (m_RetryCount == 0)
                            {
                                onError?.Invoke(err);
                            }
                            else
                            {
                                var delayTime = m_DelayInMlliseconds * a++;
                                Thread.Sleep(TimeSpan.FromMilliseconds(delayTime));
                            }

                        }, args);

            }
        }
    }
    public enum Case
    {
        /// <summary>
        /// Performs a specified number of retries, using a specified fixed time interval between retries. 
        /// </summary>
        Retry,
        /// <summary>
        /// Performs a specified number of retry after every exponential number of delay time, delay = 10, retryCount = 3 than delay time will be 10^1,10^2 and 10^3
        /// </summary>
        Exponential,
        /// <summary>
        /// Performs a specified number of retry attempts and an incremental time interval between retries, delay will be number of retries*intervalTime. 
        /// </summary>
        Geometric
    }
}
