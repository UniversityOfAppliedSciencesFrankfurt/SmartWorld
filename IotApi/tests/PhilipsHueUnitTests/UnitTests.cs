using Iot;
using PhilipsHueConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using System.Globalization;
using System.Net.Http;
using Newtonsoft.Json;
using PhilipsHueConnector.Entities;

namespace PhilipsHueUnitTests
{
    public class UnitTests
    {

        /// <summary>
        /// URL of the Hue gateway.
        /// </summary>
         private string m_GtwUri = "http://192.168.0.109";

        /// <summary>
        /// To set username, you first have to run test GenerateUserTest().
        /// This method will connect to Hue Gateway. BEfore you run it, click 
        /// the link button on the gatewey. Method GenerateUserName will return
        /// username, which you should set as value of this member variable.
        /// </summary>
        private string m_UsrName = "x5sfUaeLQ1YE3np5E5AUhyjWkNnVxHxdFxXoWupp";

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
            (err,ex) =>
            {
                throw ex;
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
            Schedule sch = SetSchedule(m_UsrName);
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
            GetSchedules(2, m_UsrName);
            //var iotApi = getApi();
            //var result = iotApi.SendAsync(new HueCommand()
            //{ 
            //    Path = $"schedules",
            //    Method = "get",

            //}).Result;
            //Assert.IsType(typeof(Newtonsoft.Json.Linq.JArray), result);
        }

        private Schedule GetSchedules(int scnumber, string username)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.0.109");
            var res = client.GetAsync($"api/{username}/schedules/{scnumber}").Result;
            Schedule sc = null;
            if(res.IsSuccessStatusCode)
            {
                 sc= JsonConvert.DeserializeObject<Schedule>(res.Content.ReadAsStringAsync().Result);
            }
            return sc;
            
        }
        [Fact]
        public void DeleteScheduleTest()
        {
            String result = DeleteSchedule(1,m_UsrName);
            Assert.True(result != null);
        }


        public String DeleteSchedule(int id, string username)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.0.109/");
            var res = client.DeleteAsync($"api/{username}/schedules/{id}").Result;
            String sc = null;
            if (res.IsSuccessStatusCode)
            {
                sc = $"Successfully deleted id {id}";
            }
            return sc;
        }
        public Schedule SetSchedule(string username)
        {
            TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
            DateTime dateTime = DateTime.Now;
            String localTime = dateTime.ToString("yyyy-MM-ddT16:48:00");
            Schedule sc = new Schedule()
            {
                name = "New Schedule",
                description = "Turn the light on",
                command = new Command()
                {
                    address = $"/api/{username}/group/4/action",
                    method = "PUT",
                    body = new body()
                    {
                        on = true,
                        bri = 2000
                    },
                },
                localtime = localTime,
            };
            return sc;
        }

        [Fact]
        public void testSetGroup()
        {
            var iotApi = getApi();
            Group gr = SetGroup();
            var result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"groups",
                Method = "post",
                Body = gr
            }).Result;
            Assert.True(result != null);
        }
        public Group SetGroup()
        {
            Group gr = new Group()
            {
                lights = new string[] {"1","4"},
                name = "group SE",
                type = "LightGroup"
            };
            return gr;
        }

        [Fact]
        public void testSetGroupAction()
        {
            var iotApi = getApi();
            var result = iotApi.SendAsync(new HueCommand()
            {
                Path = $"groups/4/action",
                Method = "put",
                Body = new
                {
                    on = true,
                    hue = 2000,
                    effect = "colorloop"
                }
            }).Result;
            Assert.True(result != null);
        }
    }
}
