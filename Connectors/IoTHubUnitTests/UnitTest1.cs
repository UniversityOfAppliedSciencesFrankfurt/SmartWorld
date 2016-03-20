using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Azure.IotHubConnector;
using System.Collections.Generic;
using System.Configuration;

namespace IoTHubUnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void InitWithExpliciteDeviceId_Test()
        {
            IotHubConnector conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
                { "ConnStr", ConfigurationManager.AppSettings["ConnStr"] },
                { "DeviceId", ConfigurationManager.AppSettings["DeviceId"] },
            }).Wait();

            conn.SendAsync(
                new { temperature = 22.0, sensor="unittest", messageId="1"},
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
            IotHubConnector conn = new IotHubConnector();
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
            IotHubConnector conn = new IotHubConnector();
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

        private static IotHubConnector getConnector()
        {
            string conStr = $"{ConfigurationManager.AppSettings["ConnStr"]};{ConfigurationManager.AppSettings["DeviceId"]}";
            IotHubConnector conn = new IotHubConnector();
            conn.Open(new Dictionary<string, object>() {
                { "ConnStr", "DeviceId=PI2-01;HostName=DRoth-IotHub-01.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=G0ddVsZb5UYFIt2iVTnN+psldF0qRHHxKMUcAo1tdWE=" },
            }).Wait();
            return conn;
        }
    }
}
