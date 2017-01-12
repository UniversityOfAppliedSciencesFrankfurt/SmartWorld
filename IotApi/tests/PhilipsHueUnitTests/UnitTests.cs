using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using PhilipsHueConnector;

namespace PhilipsHueUnitTests
{
    public class UnitTests
    {
        private string m_GtwUri = "TODO";

        private string m_UsrName = "TODO";


        /// <summary>
        /// Used by all tests to create instance of IotApi.
        /// </summary>
        /// <returns></returns>
        private IotApi getApi()
        {
            IotApi api = new IotApi();

            api.UsePhilpsQueueRest(m_UsrName, m_GtwUri);

            api.Open();

            return api;
        }


        /// <summary>
        /// This test will try to get new random user from gateway.
        /// If the button on gateway is not pressed, test will fail after 3 retries.
        /// </summary>
        [Fact]
        public void GenerateUserTest()
        {
            var username = new IotApi().GenerateUserName(m_GtwUri);

            Assert.Throws(typeof(Exception),()=>{

            });
            //
        }

        [Fact]
        public void GetLightsTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights()).Wait();

           
        }
    }
}
