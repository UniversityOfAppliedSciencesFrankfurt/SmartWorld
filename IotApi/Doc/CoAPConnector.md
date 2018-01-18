# CoAP 
The Constrained Application Protocol (CoAP) was designed by the Constrained RESTful Environments (CoRE) work group in IEFT. 
It is a specialized web transfer protocol for use with constrained nodes and constrained networks. In the IoT, constraints on nodes 
typically emerge in terms of limited power supply, manufacturing costs, RAM, ROM, and generally low processing capabilities. 
Yet, constrained nodes, i.e., devices, are powerful enough to send and receive network packets and benefit from a connection to the Internet 
as they can be integrated into a distributed service.

## Guides
This section described how to use CoAP protocol with IotApi -

#### Get Request
```C#
var mock = new Mock<ICoapEndpoint>();
mock
    .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
    .Returns(Task.CompletedTask);

IotApi api = new IotApi()
        .UserCoAPModule(mock.Object);
api.Open();

var retult = api.SendAsync(new CoapMessage
    {
        Type = CoapMessageType.Confirmable,
        Code = CoapMessageCode.Get
    }).Result;
```
#### Post Request 
```C#
var mock = new Mock<ICoapEndpoint>();
mock
    .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
    .Returns(Task.CompletedTask);
    
IotApi api = new IotApi()
        .UserCoAPModule(mock.Object);
api.Open();
    
var result = api.SendAsync(new CoapMessage
        {
            Type = CoapMessageType.Confirmable,
            Code = CoapMessageCode.Post
        }).Result;
```
#### Delete Request
```C#
var mock = new Mock<ICoapEndpoint>();
mock
    .Setup(c => c.SendAsync(It.IsAny<CoapPayload>()))
    .Returns(Task.CompletedTask);

IotApi api = new IotApi()
        .UserCoAPModule(mock.Object);
api.Open();

api.SendAsync(new CoapMessage
    {
        Type = CoapMessageType.Confirmable,
        Code = CoapMessageCode.Delete
    }).Result;
```

You can see more examples in CoAP unit test 