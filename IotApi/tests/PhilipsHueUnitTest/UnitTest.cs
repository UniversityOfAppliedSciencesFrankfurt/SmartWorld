using Iot;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhilipsHueConnector;
using System;

namespace PhilipsHueUnitTest
{
    [TestClass]
    public class UnitTest
    {
        private string m_GtwUri = "http://192.168.0.71";

        private string m_UsrName = "";


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
            
            Assert.ThrowsException<Exception>(() =>
            {

            });
            //
        }

        [TestMethod]
        public void GetLightsTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights()).Wait();
        }
    }
}
