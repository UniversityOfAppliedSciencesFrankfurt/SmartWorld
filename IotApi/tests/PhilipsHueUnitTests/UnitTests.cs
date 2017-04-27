using Iot;
using Iot.PhilipsHueConnector.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Globalization;

namespace PhilipsHueUnitTests
{
    public class UnitTests
    {

        /// <summary>
        /// URL of the Hue gateway.
        /// </summary>
         private string m_GtwUri = "http://192.168.0.115/";

        /// <summary>
        /// To set username, you first have to run test GenerateUserTest().
        /// This method will connect to Hue Gateway. BEfore you run it, click 
        /// the link button on the gatewey. Method GenerateUserName will return
        /// username, which you should set as value of this member variable.
        /// </summary>
        private string m_UsrName = "GEX70ryKiblxzsHVWswfs4E49zuI00nnhMOBkxcH";

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
        /// WHen executing unit tests button will typically not be pressed. This is why
        /// we expect this test to fail.
        /// </summary>
        [Fact]
        public void GenerateUserTest()
        {
            Assert.Throws(typeof(IotApiException), () =>
            {
                var username = new IotApi().GenerateUserName(m_GtwUri);
            });
        }

        [Fact]
        public void GetLightsTest()
        {
            var iotApi = getApi();

            List<Device> result = iotApi.SendAsync(new GetLights()).Result as List<Device>;

            Assert.NotNull(result);

            Assert.Equal(result.Count, TestDriver.NumOfDevices);
        }


        [Fact]
        public void GetLightsJSApiStyleTest()
        {
            var iotApi = getApi();

            iotApi.SendAsync(new GetLights(), (result) =>
            {
                Assert.NotNull(result);

                Assert.IsType<List<Device>>(result);

                List<Device> res = result as List<Device>;

                Assert.Equal(res.Count, TestDriver.NumOfDevices);
            },
            (err) =>
            {
                throw err;
            }).Wait();
        }


        [Fact]
        public void SwitchOnLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightState()
            {
                Id = TestDriver.LightStateReferenceId,

                State = new State()
                {
                    On = true,
                    Bri = 254,
                },

            }).Result;

            Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);

            Assert.True(((Newtonsoft.Json.Linq.JArray)result).Count == 5);
        }


        [Fact]
        public void SetLightOnGreenTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightState()
            {
                Id = TestDriver.LightStateReferenceId,

                State = new State()
                {
                    On = true,
                    Bri = 254,
                    xy = new List<double>()
                     {
                        0.1, 0.85
                     }
                },

            }).Result;

            Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);

            Assert.True(((Newtonsoft.Json.Linq.JArray)result).Count == 6);
        }

        [Fact]
        public void SwitchOffLightTest()
        {
            var iotApi = getApi();

            var result = iotApi.SendAsync(new SetLightState()
            {
                Id = TestDriver.LightStateReferenceId,

                State = new State()
                {
                    On = false,
                },

            }).Result;

            Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);

            Assert.True(((Newtonsoft.Json.Linq.JArray)result).Count == 1);
        }


        [Fact]
        public void ToggleLightUntypedTest()
        {
            var iotApi = getApi();
            var result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"lights/{TestDriver.LightStateReferenceId}/state",
                Method = "put",
                Body = new
                {
                    on = true,
                    bri = 100
                },

            }).Result;

            Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);

            Assert.True(((Newtonsoft.Json.Linq.JArray)result).Count == 2);

            result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"lights/{TestDriver.LightStateReferenceId}/state",
                Method = "put",
                Body = new
                {
                    on = false
                },

            }).Result;

            Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);

            Assert.True(((Newtonsoft.Json.Linq.JArray)result).Count == 1);
        }
        [Fact]
        public void SetSchedulesTest()
        {
            var iotApi = getApi();
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime dateTime = DateTime.Now;
            int d = dateTime.Day;
            int m = dateTime.Month;
            int y = dateTime.Year;
            int h = dateTime.Hour;
            int min = dateTime.Minute;
            int s = dateTime.Second;
            String Time = "";
            String localTime = y + "-" + m + "-" + d + "T" + h + ":" + min + ":" + s;
            //Console.WriteLine(localTime);
            //Console.ReadLine();
            Schedule sch = new Schedule()
            {
                name = "wake up",
                description = "wake up",
                command = new Command()
                {
                    address = "/api/GEX70ryKiblxzsHVWswfs4E49zuI00nnhMOBkxcH/schedules",
                    method = "POST",
                    body = new body()
                    {
                        On = true
                    },
                },
                localtime = localTime,  
            };

            var result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"schedules",
                Method = "post",
                Body = sch
            }).Result;
            Assert.True(result != null);
        }
        [Fact]
        public void GetScheduleTest()
        {
            var iotApi = getApi();
            var result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"schedules",
                Method = "get",

            }).Result;
            Assert.True(result != null);

        }
    }
}
