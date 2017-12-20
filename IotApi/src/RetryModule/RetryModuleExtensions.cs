using Iot;
using System;
using System.Collections.Generic;
using System.Text;

namespace RetryModule
{
    public static class RetryModuleExtensions
    {
        /// <summary>
        /// Extension method for Retry module 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="selectedCase"></param>
        /// <param name="retryCount">How many times want to try</param>
        /// <param name="delayInMiliSecond">Delay time in milliseconds</param>
        /// <returns></returns>
        public static IotApi UseRetryModule(this IotApi api, Case selectedCase, int retryCount, int delayInMlliseconds)
        {
            api.RegisterModule(new RetryModuleConnector(selectedCase, retryCount, delayInMlliseconds));
            return api;
        }
    }
}
