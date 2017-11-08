using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoAPConnector;
using System.Text;
using Moq;
using Xunit;
//using TestToolsToXunitProxy;


namespace CoAPUnitTest
{
    /// <summary>
    /// Description
    /// </summary>
    /// Unit test classes
    public class UnitTests
    {
        #region member variables
        public readonly int m_MaxTaskTimeout = System.Diagnostics.Debugger.IsAttached ? -1 : 2000;
        #endregion

        /// <summary>
        /// Register CoAP module to IotApi module
        /// </summary>
        /// <param name="mock">read in a Mock Object of ICoAPEndpoint</param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        private IotApi getApi(Mock<ICoapEndpoint> mock)
        {
            Dictionary<string, object> agr = new Dictionary<string, object>();
            IotApi api = new IotApi().RegisterModule(new CoAPConnector.CoAPclientConnector());
            agr.Add("endPoint", mock.Object);
            api.Open(agr);
            return api;
        }

        /// <summary>
        /// Test Get Request
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
        public void TestclientRequestGet()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Get
            }
            ).Wait();

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Post Request
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
        public void TestclientRequestPost()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Post
            }
            ).Wait();

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Delete Request
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
        public void TestclientRequestDelete()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            getApi(mock).SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Delete
            }
            ).Wait();

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test Put Request
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
        public void TestclientRequestPut()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                .Returns(Task.CompletedTask);

            // Act
            var api = getApi(mock);
            api.SendAsync(new CoapMessage
            {
                Type = CoapMessageType.Confirmable,
                Code = CoapMessageCode.Put
            }
            ).Wait();

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        /// <summary>
        /// Test receive response from GET request
        /// </summary>
        /// <param name=""></param>
        /// <exception cref=""></exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
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
            api.ReceiveAsync(agr).Wait();

            // Assert
            mock.Verify(x => x.ReceiveAsync(), Times.AtLeastOnce);
        }

        /// <summary>
        /// Test receiving multicast message from multicast endpoint
        /// </summary>
        /// <param name=""></param>
        /// <exception cref="CoapEndpointException">Close CoAP Endpoint</exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
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
            Assert.True(messageReceived.Task.IsCompleted, "Took too long to receive message");
            Assert.True(messageReceived.Task.Result, "Message is not marked as Multicast");
        }

        /// <summary>
        /// Test Non-confirmable message
        /// </summary>
        /// <param name=""></param>
        /// <exception cref="CoapEndpointException">Close CoAP endpoint</exception>
        /// <remarks>N/a</remarks>
        /// <seealso cref=""/>
        /// <see cref=""/>
        /// <permission cref="">This method can be called by: every user</permission>
        /// <exception cref=""></exception>
        [Fact]
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
            Assert.True(closedEventSource.Task.IsCompleted, "Took too long to receive message");
            mockclientEndpoint.Verify(x => x.SendAsync(It.IsAny<CoapPayload>()), Times.Never, "Multicast Message was responded to whenn it shouldn't");
        }

        //[Fact]
        //public void TestclientOnMessageReceivedEvent()
        //{
        //    // Arrange
        //    var expected = new CoapMessage
        //    {
        //        Id = 0x1234,
        //        Type = CoapMessageType.Acknowledgement,
        //        Code = CoapMessageCode.None,
        //    };
        //    var mockclientEndpoint = new Mock<ICoapEndpoint>();

        //    mockclientEndpoint
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = expected.Serialise()
        //        }))
        //        .Throws(new CoapEndpointException("Endpoint closed"));

        //    // Act
        //    var test = new CoAPclientConnector();
        //    Dictionary<string, object> agr = new Dictionary<string, object>();
        //    IotApi api = new IotApi().RegisterModule(new CoAPConnector.CoAPclientConnector());
        //    agr.Add("endPoint", mockclientEndpoint.Object);
        //    api.Open(agr);

        //    bool result = test.listenertest(mockclientEndpoint);

        //    // Assert
        //    Assert.True(result);
        //}



        //[Fact]
        //public void TestRejectEmptyMessageWithFormatError()
        //{
        //    // Arrange
        //    var expected = new CoapMessage
        //    {
        //        Id = 0x1234,
        //        Type = CoapMessageType.Reset,
        //        Code = CoapMessageCode.None,
        //    };

        //    var mockclientEndpoint = new Mock<ICoapEndpoint>();
        //    mockclientEndpoint
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new byte[] { 0x40, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Confirmable Message with a payload
        //        }))
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new byte[] { 0x60, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Acknowledge Message with a payload (ignored)
        //        }))
        //        .Throws(new CoapEndpointException("Endpoint closed"));

        //    // Act
        //    using (var mockclient = new Coapclient(mockclientEndpoint.Object))
        //    {
        //        mockclient.Listen();

        //        // Assert
        //        mockclientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(expected.Serialise()))),
        //            Times.Exactly(1));
        //    }
        //}


        //[Fact]
        //public void TestRequestWithSeperateResponse()
        //{
        //    // Arrange
        //    var token = new byte[] { 0xC0, 0xFF, 0xEE };
        //    var requestMessage = new CoapMessage
        //    {
        //        Id = 0x1234,
        //        Token = token,
        //        Type = CoapMessageType.Confirmable,
        //        Code = CoapMessageCode.Get,
        //        Options = new System.Collections.Generic.List<CoapOption>
        //        {
        //            new CoAPConnector.Options.UriPath("test")
        //        }
        //    };

        //    var acknowledgeMessage = new CoapMessage
        //    {
        //        Id = 0xfeed,
        //        Type = CoapMessageType.Acknowledgement,
        //        Token = token
        //    };

        //    var mockclientEndpoint = new Mock<ICoapEndpoint>();
        //    mockclientEndpoint
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new CoapMessage
        //            {
        //                Id = 0x1234,
        //                Token = token,
        //                Type = CoapMessageType.Acknowledgement
        //            }.Serialise()
        //        }))
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new CoapMessage
        //            {
        //                Id = 0xfeed,
        //                Token = token,
        //                Type = CoapMessageType.Confirmable,
        //                Code = CoapMessageCode.Content,
        //                Payload = Encoding.UTF8.GetBytes("Test Resource")
        //            }.Serialise()
        //        }))
        //        .Throws(new CoapEndpointException("Endpoint closed"));

        //    // Act
        //    using (var mockclient = new Coapclient(mockclientEndpoint.Object))
        //    {
        //        mockclient.OnMessageReceived += (s, e) =>
        //        {
        //            mockclient.SendAsync(acknowledgeMessage).Wait(m_MaxTaskTimeout);
        //        };

        //        var requestTask = mockclient.SendAsync(requestMessage);
        //        requestTask.Wait(m_MaxTaskTimeout);
        //        if (!requestTask.IsCompleted)
        //            throw new NUnit.Framework.AssertionException("Took too long to send Get request");


        //        mockclient.Listen();

        //        var reponseTask = mockclient.GetResponseAsync(requestTask.Result);
        //        reponseTask.Wait(m_MaxTaskTimeout);
        //        if (!reponseTask.IsCompleted)
        //            throw new NUnit.Framework.AssertionException("Took too long to get reponse");

        //        // Assert
        //        mockclientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(requestMessage.Serialise()))),
        //            Times.Exactly(1));

        //        mockclientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(acknowledgeMessage.Serialise()))),
        //            Times.Exactly(1));
        //    }
        //}
    }
}

