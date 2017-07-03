using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoAPConnector;
using Moq;
using Xunit;


namespace CoAPUnitTest
{
    public class Client
    {
        /// Timout for any Tasks
        public readonly int MaxTaskTimeout = System.Diagnostics.Debugger.IsAttached ? -1 : 2000;
        // Every task use this
        private IotApi getApi(Mock<ICoapEndpoint> mock)
        {
            Dictionary<string, object> agr = new Dictionary<string, object>();
            IotApi api = new IotApi().RegisterModule(new CoAPConnector.CoAPClientConnector());
            agr.Add("endPoint", mock.Object);
            api.Open(agr);
            return api;
        }

        // Test num 1
        [Fact]
        public void TestClientRequest()
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
                Code = CoapMessageCode.Get
            }
            ).Wait();

            // Assert
            mock.Verify(cep => cep.SendAsync(It.IsAny<CoapPayload>()));
        }

        // test num 2
        [Fact]
        public void TestClientResponse()
        {
            // Arrange
            var mock = new Mock<ICoapEndpoint>();
            var mockPayload = new Mock<CoapPayload>();

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

            mockPayload
                .Setup(p => p.Payload)
                .Returns(() => expected.Serialise());
            mock
                .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
                // Copy the ID from the message sent out, to the message for the client to receive
                .Callback<CoapPayload>((p) => expected.Id = p.MessageId)
                .Returns(Task.CompletedTask);
            mock
                .SetupSequence(c => c.ReceiveAsync())
                .Returns(Task.FromResult(mockPayload.Object))
                .Throws(new CoapEndpointException("Endpoint closed"));

            // Ack
            getApi(mock).ReceiveAsync().Wait();
 
            // Assert
            mock.Verify(x => x.ReceiveAsync(), Times.AtLeastOnce);
        }


        // test num 3
        //[Fact]
        //public void TestClientOnMessageReceivedEvent()
        //{
        //    // Arrange
        //    var expected = new CoapMessage
        //    {
        //        Id = 0x1234,
        //        Type = CoapMessageType.Acknowledgement,
        //        Code = CoapMessageCode.None,
        //    };
        //    var mock = new Mock<ICoapEndpoint>();
        //    var clientOnMessageReceivedEventCalled = false;

        //    mock
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = expected.Serialise()
        //        }))
        //        .Throws(new CoapEndpointException("Endpoint closed"));

        //    // Act


        //    var task = new TaskCompletionSource<bool>();
        //        //getApi(mock).OnMessageReceived += (s, e) =>
        //        {
        //            clientOnMessageReceivedEventCalled = true;
        //            task.SetResult(true);
        //        };

        //        //mockClient.Listen();

        //        task.Task.Wait(MaxTaskTimeout);
        //    }

        //    // Assert
        //    //Assert.IsTrue(clientOnMessageReceivedEventCalled);
        //}


        // test num 4
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

        //    var mock = new Mock<ICoapEndpoint>();
        //    mock
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new byte[] { 0x40, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Confirmable Message with a payload
        //            }))
        //        .Returns(Task.FromResult(new CoapPayload
        //        {
        //            Payload = new byte[] { 0x60, 0x00, 0x12, 0x34, 0xFF, 0x12, 0x34 } // "Empty" Acknowledge Message with a payload (ignored)
        //            }))
        //        .Throws(new CoapEndpointException("Endpoint closed"));

        //    // Act
        //    using (var mockClient = new CoapClient(mockClientEndpoint.Object))
        //        mockClient.Listen();

        //        // Assert
        //        mockClientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(expected.Serialise()))),
        //            Times.Exactly(1));
        //}

        // test num 5
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
        //            new Options.UriPath("test")
        //        }
        //    };

        //    var acknowledgeMessage = new CoapMessage
        //    {
        //        Id = 0xfeed,
        //        Type = CoapMessageType.Acknowledgement,
        //        Token = token
        //    };

        //    var mockClientEndpoint = new Mock<ICoapEndpoint>();
        //    mockClientEndpoint
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
        //    using (var mockClient = new CoapClient(mockClientEndpoint.Object))
        //    {
        //        mockClient.OnMessageReceived += (s, e) =>
        //        {
        //            mockClient.SendAsync(acknowledgeMessage).Wait(MaxTaskTimeout);
        //        };

        //        var requestTask = mockClient.SendAsync(requestMessage);
        //        requestTask.Wait(MaxTaskTimeout);
        //        if (!requestTask.IsCompleted)
        //            Assert.Fail("Took too long to send Get request");


        //        mockClient.Listen();

        //        var reponseTask = mockClient.GetResponseAsync(requestTask.Result);
        //        reponseTask.Wait(MaxTaskTimeout);
        //        if (!reponseTask.IsCompleted)
        //            Assert.Fail("Took too long to get reponse");

        //        // Assert
        //        mockClientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(requestMessage.Serialise()))),
        //            Times.Exactly(1));

        //        mockClientEndpoint.Verify(
        //            cep => cep.SendAsync(It.Is<CoapPayload>(p => p.Payload.SequenceEqual(acknowledgeMessage.Serialise()))),
        //            Times.Exactly(1));
        //    }
        //}

        //test num 6
        //public void TestMulticastMessagFromMulticastEndpoint()
        //{
        //    // Arrange
        //    var mockClientEndpoint = new Mock<ICoapEndpoint>();
        //    var mockPayload = new Mock<CoapPayload>();

        //    var messageReceived = new TaskCompletionSource<bool>();

        //    var expected = new CoapMessage
        //    {
        //        Type = CoapMessageType.NonConfirmable,
        //        Code = CoapMessageCode.Get,
        //        Options = new System.Collections.Generic.List<CoapOption>
        //            {
        //                new Options.ContentFormat(Options.ContentFormatType.ApplicationLinkFormat)
        //            },
        //        Payload = Encoding.UTF8.GetBytes("</.well-known/core>")
        //    };

        //    mockPayload
        //        .Setup(p => p.Payload)
        //        .Returns(() => expected.Serialise());

        //    mockClientEndpoint.Setup(c => c.IsMulticast).Returns(true);
        //    mockClientEndpoint
        //        .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
        //        .Returns(Task.CompletedTask);
        //    mockClientEndpoint
        //        .SetupSequence(c => c.ReceiveAsync())
        //        .Returns(Task.FromResult(mockPayload.Object))
        //        .Throws(new CoapEndpointException("Endpoint closed"));


        //    // Ack
        //    using (var client = new CoapClient(mockClientEndpoint.Object))
        //    {
        //        client.OnMessageReceived += (s, e) => messageReceived.SetResult(e?.Message?.IsMulticast ?? false);
        //        client.Listen(); // enable loop back thingy

        //        messageReceived.Task.Wait(MaxTaskTimeout);
        //    }

        //    // Assert
        //    Assert.IsTrue(messageReceived.Task.IsCompleted, "Took too long to receive message");
        //    Assert.IsTrue(messageReceived.Task.Result, "Message is not marked as Multicast");
        //}

        //test num 7
        //public void TestMulticastMessageIsNonConfirmable()
        //{
        //    // Arrange
        //    var mockClientEndpoint = new Mock<ICoapEndpoint>();
        //    var closedEventSource = new TaskCompletionSource<bool>();

        //    mockClientEndpoint.Setup(c => c.IsMulticast).Returns(true);
        //    mockClientEndpoint
        //        .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
        //        .Returns(Task.CompletedTask);
        //    mockClientEndpoint
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


        //    // Ack
        //    using (var client = new CoapClient(mockClientEndpoint.Object))
        //    {
        //        client.OnClosed += (s, e) => closedEventSource.SetResult(true);
        //        client.Listen(); // enable loop back thingy

        //        closedEventSource.Task.Wait(MaxTaskTimeout);
        //    }

        //    // Assert
        //    Assert.IsTrue(closedEventSource.Task.IsCompleted, "Took too long to receive message");
        //    mockClientEndpoint.Verify(x => x.SendAsync(It.IsAny<CoapPayload>()), Times.Never, "Multicast Message was responded to whenn it shouldn't");
        //}

    }
}
