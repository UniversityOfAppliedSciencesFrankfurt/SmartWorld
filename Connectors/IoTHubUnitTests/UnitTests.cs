using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using Daenet.Iot;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using System.Text;

namespace IoTHubUnitTests
{
    [TestClass]
    public class UnitTests
    {

        private static IotHubConnector getConnector()
        {
            string conStr = $"{ConfigurationManager.AppSettings["ConnStr"]};{ConfigurationManager.AppSettings["DeviceId"]}";
            IotHubConnector conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
                { "ConnStr",conStr }
            }).Wait();
            return conn;
        }


        private static Microsoft.Azure.Devices.Message createServiceMessage(object sensorMessage)
        {
            var messageString = JsonConvert.SerializeObject(sensorMessage);

            var message = new Microsoft.Azure.Devices.Message(Encoding.UTF8.GetBytes(messageString));

            return message;
        }

        [TestMethod]
        public void InitWithExpliciteDeviceId_Test()
        {
            IotHubConnector conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
                { "ConnStr", ConfigurationManager.AppSettings["ConnStr"] },
                { "DeviceId", ConfigurationManager.AppSettings["DeviceId"] },
            }).Wait();

            conn.SendAsync(
                new { temperature = 22.0, sensor = "unittest", messageId = "1" },
                (msgs) =>
                {
                    Assert.IsTrue(msgs.Count == 1);
                    dynamic msg = msgs[0];
                    Assert.IsTrue(msg.messageId == "1");
                },

                (msgs, err) =>
                {
                    throw err;
                }).Wait();
        }


        [TestMethod]
        public void InitWithDeviceIdInConnStr_Test()
        {
            string conStr = $"{ConfigurationManager.AppSettings["ConnStr"]};DeviceId={ConfigurationManager.AppSettings["DeviceId"]}";
            IIotApi conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
                 { "ConnStr", conStr },
            }).Wait();

            conn.SendAsync(
             new { temperature = 22.0, sensor = "unittest", messageId = "123" },
             (msgs) =>
             {
                 Assert.IsTrue(msgs.Count == 1);
                 dynamic msg = msgs[0];
                 Assert.IsTrue(msg.messageId == "123");
             },

             (msgs, err) =>
             {
                 throw err;
             }).Wait();
        }


        [TestMethod]
        public void SendBatch_Test()
        {
            IIotApi conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
               { "ConnStr", ConfigurationManager.AppSettings["ConnStr"] },
               { "DeviceId", ConfigurationManager.AppSettings["DeviceId"] },
               { "NumOfMessagesPerBatch", 100 }
            }).Wait();


            for (int i = 0; i < 100; i++)
            {
                conn.SendAsync(
                 new { temperature = 22.0, sensor = "unittest", messageId = "123" },
                 (msgs) =>
                 {
                     Assert.IsTrue(msgs.Count == 100);
                     dynamic msg = msgs[0];
                     Assert.IsTrue(msg.messageId == "123");
                 },

                 (msgs, err) =>
                 {
                     throw err;
                 }).Wait();
            }
        }

        [TestMethod]
        public void ReceiveBatch_Test()
        {
            string deviceId = ConfigurationManager.AppSettings["DeviceId"];

            IIotApi conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
               { "ConnStr", ConfigurationManager.AppSettings["ConnStr"] },
               { "DeviceId", deviceId },
               { "NumOfMessagesPerBatch", 100 }
            }).Wait();

            ServiceClient svcClient = ServiceClient.CreateFromConnectionString(ConfigurationManager.AppSettings["ServiceConnStr"]);

            //svcClient.SendAsync(deviceId, createServiceMessage(new { Command = "testrcv", Value = "msg1" })).Wait();

            conn.ReceiveAsync((msg) =>
            {
               dynamic obj = JsonConvert.DeserializeObject(Encoding.UTF8.GetString((byte[])msg));

                Assert.IsTrue(obj.Command == "testrcv");

                Assert.IsTrue(obj.Value == "msg1");
            },
            (err) => 
            {
                throw err;
            }, 0).Wait();
        }
    }
}
