using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot;
using ProfinetConnector;
using Dacs7.Domain;

namespace ProfinetConnectorUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SendMessageTest()
        {
            int length = 500;
            byte[] testData = new byte[500];
            int offset = 10; //The offset is the number of bytes from the beginning of the area
            int dbNumber = 560;

            var mgs = new SensorMessage()
            {
                Area = PlcArea.DB,
                Offset = offset,
                Value = testData,
                Args = new[] { length, dbNumber }

            };

            var api = getIotApi();

            api.SendAsync(mgs,(succ)=> {

                var succMgs = succ;

            },(obj,err)=> {

                var error = err;

            }).Wait();
        }
        
        private IotApi getIotApi()
        {
            var api = new IotApi()
                .UseProfinet("Data Source=127.0.0.1:102,0,2");
            api.Open();

            return api;
        }
    }
}
