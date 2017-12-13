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

        /// <summary>
        /// Get list of lights which connected with Philips hue gateway
        /// </summary>
        [TestMethod]
        public void GetLightTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights()).Wait();
        }

        /// <summary>
        /// Get list of lights which connected with Philips hue gateway as a list of devices 
        /// </summary>
        [TestMethod]
        public void GetLightsTest()
        {
            var iotApi = getApi();

            List<Device> result = iotApi.SendAsync(new GetLights()).Result as List<Device>;

            Assert.IsNotNull(result);

            Assert.AreEqual(result.Count, TestDriver.NumOfDevices);
        }

        /// <summary>
        /// Get list of lights which connected with Philips hue gateway using api style 
        /// </summary>
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

        /// <summary>
        /// Get new lights as list of devices
        /// </summary>
        [TestMethod]
        public void GetNewLightTest()
        {
            var api = getApi();
            var result = api.SendAsync(new GetNewLight()).Result as List<Device>;

            Assert.IsTrue(result.Count == 0);
        }

        /// <summary>
        /// Get light states for example, light is on/off, color values and so on
        /// </summary>
        [TestMethod]
        public void GetLisghtStatesTest()
        {
            var api = getApi();

            var result = api.SendAsync(new GetLightStates()
            {
                Id = "4"
            }).Result;

            Assert.IsTrue(result != null);
        }

        /// <summary>
        /// Switch off the light
        /// </summary>
        [TestMethod]
        public void SwitchOffLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                Body = new State()
                {
                    on = false
                },

            }).Result;

            Assert.IsTrue(result is JArray);
        }

        /// <summary>
        /// Switch on the light
        /// </summary>
        [TestMethod]
        public void SwitchOnLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                Body = new State()
                {
                    on = true,
                    bri = 120
                },

            }).Result;

            Assert.IsTrue(result is JArray);
        }

        /// <summary>
        /// Set light colors
        /// </summary>
        [TestMethod]
        public void SetLightColorChangeTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = "4",

                Body = new State()
                {
                    on = true,
                    bri = 120,
                    xy = new List<double>()
                     {
                        0.692, 0.308
                     }
                },

            }).Result;

            Assert.IsTrue(result is JArray);
        }

        /// <summary>
        /// Change attributes of light for example, light name
        /// </summary>
        [TestMethod]
        public void SetLightAttributesTest()
        {
            var api = getApi();
            var result = api.SendAsync(new SetLightAttributes()
            {
                Id = "1",
                Body = new
                {
                    name = "Bedroom Light"
                }
            }).Result;

            Assert.IsTrue(result is JArray);
        }

        /// <summary>
        /// Search new light connected with Philips hue gateway
        /// </summary>
        [TestMethod]
        public void SearchForNewLightTest()
        {
            var api = getApi();
            var result = api.SendAsync(new SerarchNewLights()
            {
                Body = new
                {
                    deviceid = new []{"45AF34","543636","34AFBE" }
                }
            }).Result;

            Assert.IsTrue(result is JArray);
        }
    }
}
