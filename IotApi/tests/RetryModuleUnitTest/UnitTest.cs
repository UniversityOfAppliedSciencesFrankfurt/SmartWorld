using Iot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RetryModule;
using System;
using System.Collections.Generic;

namespace RetryModuleUnitTest
{
    [TestClass]
    public class UnitTest
    {
        [TestMethod]
        public void PerformRetryTest()
        {
            string selectedCase = "Retry";
            var api = getIotApi(selectedCase, 5, 1000);
            api.UseDummyModule();
            api.Open();
            try
            {
                api.SendAsync("my message",(succ)=>
                {
                    var s = succ;
                },(obj,err)=>
                {
                    var r = err;
                }).Wait();

            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex == null);
            }

        }

        [TestMethod]
        public void PerformRetryExponentialTest()
        {
            string selectedCase = "Exponential";

            IotApi connector = getIotApi(selectedCase, 5, 1000);

            try
            {
                connector.SendAsync("my message").Wait();
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex == null);
            }

        }

        [TestMethod]
        public void PerformRetryGeometricalTest()
        {
            string selectedCase = "Geometric";

            IotApi connector = getIotApi(selectedCase, 5, 1000);
            try
            {
                connector.SendAsync("my message").Wait();

            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex == null);
            }

        }

        private IotApi getIotApi(string selectCase, int retryCount, int delay)
        {
            var api = new IotApi();
            api.UseRetryModule(selectCase, retryCount, delay);
            return api;
        }
    }
}
