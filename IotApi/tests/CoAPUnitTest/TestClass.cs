using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoAPConnector;
using System.Text;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoAPUnitTest
{
    [TestClass]
    public class UnitTests
    {
        #region member variables
        public readonly int m_MaxTaskTimeout = System.Diagnostics.Debugger.IsAttached ? -1 : 2000;
        #endregion

        /// <summary>
        /// Register CoAP module to IotApi module
        /// </summary>
        private IotApi getApi(Mock<ICoapEndpoint> mock)
        {
            IotApi api = new IotApi()
                .UserCoAPModule(mock.Object);
            api.Open();
            return api;
        }

        /// <summary>
        /// Test Get Request
        /// </summary>
        [TestMethod]
        public void TestclientRequestGet()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            var retult = api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Get
            }
            ).Result;

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Post Request
        /// </summary>
        [TestMethod]
        public void TestclientRequestPost()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            var result = api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Post
            }
            ).Result;

            Assert.IsNotNull(result);
            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Delete Request
        /// </summary>
        [TestMethod]
        public void TestclientRequestDelete()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = getApi(mock).SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Delete
            }
            ).Result;

            Assert.IsNotNull(result);
            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Put Request
        /// </summary>
        [TestMethod]
        public void TestclientRequestPut()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            var result = api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Put
            }
            ).Result;

            Assert.IsNotNull(result);
            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test receive response from GET request
        /// </summary>
        [TestMethod]
        public void TestclientResponseGet()
        {
            // Arrange
            var expected = new CoapMessage
            {
                Type = CoapMessageType.Acknowledgement,
                Code = CoapMessageCode.Content,
                Options = new System.Collections.Generic.List<CoapOption>
                    {
                        new CoAPConnector.Options.ContentFormat(CoAPConnector.Options.ContentFormatType.ApplicationLinkFormat)
                    },
                Payload = System.Text.Encoding.UTF8.GetBytes("</.well-known/core>")
            };

            var mockPayload = new Mock<CoapPayload>();
            mockPayload
                .Setup(p => p.Payload)
                .Returns(() => expected.Serialise());

            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                // Copy the ID from the message sent out, to the message for the m_client to receive
                .Callback<CoapPayload>((p) => expected.Id = p.MessageId)
                .Returns(Task.CompletedTask);
            mock
                .SetupSequence(c => c.ReceiveAsync())
                .Returns(Task.FromResult(mockPayload.Object))
                .Throws(new CoapEndpointException("Endpoint closed"));

            // Act
            var api = getApi(mock);
            Dictionary<string, object> agr = new Dictionary<string, object>();
            string uri = "coap://example.com/.well-known/core";
            agr.Add("URI", uri);

            var result = api.ReceiveAsync(agr).Result;

            Assert.IsNotNull(result);
            // Assert
            mock.Verify(x => x.ReceiveAsync(), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test receiving multicast message from multicast endpoint
        /// </summary>
        [TestMethod]
        public void TestMulticastMessagFromMulticastEndpoint()
        {
            // Arrange
            var mockclientEndpoint = new Mock<ICoapEndpoint>();
            var mockPayload = new Mock<CoapPayload>();

            var messageReceived = new TaskCompletionSource<bool>();

            var expected = new CoapMessage
            {
                Type = CoapMessageType.NonConfirmable,
                Code = CoapMessageCode.Get,
                Options = new System.Collections.Generic.List<CoapOption>
                    {
                        new CoAPConnector.Options.ContentFormat(CoAPConnector.Options.ContentFormatType.ApplicationLinkFormat)
                    },
                Payload = Encoding.UTF8.GetBytes("</.well-known/core>")
            };

            mockPayload
                .Setup(p => p.Payload)
                .Returns(() => expected.Serialise());

            mockclientEndpoint.Setup(c => c.IsMulticast).Returns(true);
            mockclientEndpoint
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);
            mockclientEndpoint
                .SetupSequence(c => c.ReceiveAsync())
                .Returns(Task.FromResult(mockPayload.Object))
                .Throws(new CoapEndpointException("Endpoint closed"));


            // Ack
            using (var m_client = new Coapclient(mockclientEndpoint.Object))
            {
                m_client.OnMessageReceived += (s, e) => messageReceived.SetResult(e?.m_Message?.IsMulticast ?? false);
                m_client.Listen(); // enable loop back thingy

                messageReceived.Task.Wait(m_MaxTaskTimeout);
            }

            // Assert
            Assert.IsTrue(messageReceived.Task.IsCompleted, "Took too long to receive message");
            Assert.IsTrue(messageReceived.Task.Result, "Message is not marked as Multicast");
        }

        /// <summary>
        /// Test Non-confirmable message
        /// </summary>
        [TestMethod]
        public void TestMulticastMessageIsNonConfirmable()
        {
            // Arrange
            var mockclientEndpoint = new Mock<ICoapEndpoint>();
            var closedEventSource = new TaskCompletionSource<bool>();

            mockclientEndpoint.Setup(c => c.IsMulticast).Returns(true);
            mockclientEndpoint
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);
            mockclientEndpoint
                .SetupSequence(c => c.ReceiveAsync())
                .Returns(Task.FromResult(new CoapPayload
                {
                    Payload = new byte[] { 0x40, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Confirmable Message with a payload
                }))
                .Returns(Task.FromResult(new CoapPayload
                {
                    Payload = new byte[] { 0x60, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Acknowledge Message with a payload (ignored)
                }))
                .Throws(new CoapEndpointException("Endpoint closed"));


            // Ack
            using (var m_client = new Coapclient(mockclientEndpoint.Object))
            {
                m_client.OnClosed += (s, e) => closedEventSource.SetResult(true);
                m_client.Listen(); // enable loop back thingy

                closedEventSource.Task.Wait(m_MaxTaskTimeout);
            }

            // Assert
            Assert.IsTrue(closedEventSource.Task.IsCompleted, "Took too long to receive message");
            mockclientEndpoint.Verify(x => x.SendAsync(It.IsAny<CoapPayload>()), Times.Never, "Multicast Message was responded to whenn it shouldn't");
        }
    }
}

