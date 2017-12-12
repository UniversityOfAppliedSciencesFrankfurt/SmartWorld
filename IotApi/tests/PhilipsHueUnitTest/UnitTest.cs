using Iot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using PhilipsHueConnector;
using PhilipsHueConnector.Entities;
using System;
using System.Collections.Generic;

namespace PhilipsHueUnitTest
{
    [TestClass]
    public class UnitTest
    {
        private string m_GtwUri = "http://192.168.0.71";

        private string m_UsrName = "gusp-xLeBhYznPCkz0ZQBnuZ25f3cOwRpW3tiQ8k";


        /// <summary>
        /// Used by all tests to create instance of IotApi.
        /// </summary>
        /// <returns></returns>
        private IotApi getApi()
        {

            IotApi api = new IotApi();

            api.UsePhilpsQueueRest(m_GtwUri, m_UsrName);

            api.Open();

            return api;
        }

        /// <summary>
        /// This test will try to get new random user from gateway.
        /// If the button on gateway is not pressed, test will fail after 3 retries.
        /// </summary>
        [TestMethod]
        public void GenerateUserTest()
        {
           
           var username = new IotApi().GenerateUserName(m_GtwUri);

            Assert.IsTrue(!string.IsNullOrEmpty(username));
        }

        [TestMethod]
        public void GetLightTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights()).Wait();
        }

        [TestMethod]
        public void GetLightsTest()
        {
            var iotApi = getApi();

            List<Device> result = iotApi.SendAsync(new GetLights()).Result as List<Device>;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Count, TestDriver.NumOfDevices);
        }

        [TestMethod]
        public void GetLightsJSApiStyleTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights(), (result) =>
            {
                Assert.IsNotNull(result);
                
                Assert.IsTrue(result is List<Device>);

                List<Device> res = result as List<Device>;

                Assert.AreEqual(res.Count, TestDriver.NumOfDevices);
            },
            (err, ex) =>
            {
                throw ex;
            }).Wait();
        }

        [TestMethod]
        public void GetLisghtStates()
        {
            var api = getApi();

            var result = api.SendAsync(new GetLightStates()
            {
                Id = "4"
            }).Result;

            Assert.IsTrue(result != null);
        }

        [TestMethod]
        public void SwitchOffLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                State = new State()
                {
                    on = false
                },

            }).Result;

            Assert.IsTrue(result is JArray);
        }

        [TestMethod]
        public void SwitchOnLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                State = new State()
                {
                    on = true,
                    bri = 120
                },

            }).Result;

            Assert.IsTrue(result is JArray);
        }

        [TestMethod]
        public void SetLightOnGreenTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                State = new State()
                {
                    on = true,
                    bri = 254,
                    xy = new List<double>()
                     {
                        0.1, 0.85
                     }
                },

            }).Result;

            Assert.IsTrue(result is JArray);

            Assert.IsTrue(((JArray)result).Count == 6);
        }
    }
}
