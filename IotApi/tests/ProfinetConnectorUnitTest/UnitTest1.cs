using Microsoft.VisualStudio.TestTools.UnitTesting;
using Iot;
using ProfinetConnector;
using Dacs7.Domain;
using System.Collections.Generic;
using System;
using System.Linq;

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

            api.SendAsync(mgs, (succ) =>
            {

                var succMgs = succ;

            }, (obj, err) =>
            {

                var error = err;

            }).Wait();
        }


        [TestMethod]
        public void ReadMessageTest()
        {
            var length = 1;
            var offset = 1; //For bitoperations we need to specify the offset in bits  (byteoffset * 8 + bitnumber)
            var dbNumber = 1;

            var rMgs = new Dictionary<string, object>();
            rMgs.Add("readMessage", new ReadMessage()
            {
                Area = PlcArea.DB,
                Offset = offset * 8,
                Type = typeof(bool),
                Args = new int[] { length, dbNumber }
            });

            var api = getIotApi();

            var result = api.ReceiveAsync(rMgs).Result;
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
