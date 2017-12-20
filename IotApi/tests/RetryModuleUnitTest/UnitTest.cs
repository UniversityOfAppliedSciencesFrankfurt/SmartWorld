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
            var api = getIotApi(Case.Retry, 5, 1000);

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
        public void PerformRetryWithExTest()
        {
            var api = getIotApi(Case.Retry, 5, 1000);

            api.SendAsync("", (succ) =>
                {
                    var s = succ;
                }, (obj, err) =>
                {
                    var r = err;
                    Assert.IsNotNull(r);
                }).Wait();
                
        }

        [TestMethod]
        public void PerformRetryExponentialTest()
        {
            IotApi connector = getIotApi(Case.Exponential, 5, 1000);
            
            var result = connector.SendAsync("my message").Result;

            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void PerformRetryExponentialWithExTest()
        {
            var api = getIotApi(Case.Exponential, 3, 10);

            api.SendAsync("", (succ) =>
            {
                var s = succ;
            }, (obj, err) =>
            {
                var r = err;
                Assert.IsNotNull(r);
            }).Wait();

        }

        [TestMethod]
        public void PerformRetryGeometricalTest()
        {
            IotApi connector = getIotApi(Case.Geometric, 5, 10);
            
            var result = connector.SendAsync("my message").Result;

            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void PerformRetryGeometricalWithExTest()
        {
            IotApi connector = getIotApi(Case.Geometric, 3, 10);

            connector.SendAsync("",
                (succ) =>{
                    var s = succ;
                },
                (k,e)=>{
                    var er = e;
                    Assert.IsNotNull(er);
                }
                ).Wait();
        }

        private IotApi getIotApi(Case selectCase, int retryCount, int delay)
        {
            var api = new IotApi();
            api.UseRetryModule(selectCase, retryCount, delay);
            api.UseDummyModule();
            api.Open();
            return api;
        }
    }
}
