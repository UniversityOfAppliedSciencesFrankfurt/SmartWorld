# XmlRpc 
You can send the message to endpoint which supported XML-RPC protocol. We focuses on providing comfortable way for users to change
 the door settings of their rooms with the help of Dimmaktor Sensor via the HomeMatic Central Control Unit (CCU2).
## Send Message
Bellow describe how to send the message to endpoint - 

### Get the Door Status 
```C#
var api = new IotApi()
                .UseXmlRpc("CCU uri");
api.Open();

var result = api.SendAsync(new MethodCall()
  {
      MethodName = "getValue",
      SendParams = new System.Collections.Generic.List<Param>() {
          new Param()
          {
              Value = "LEQ1335713:1"
          },
          new Param()
          {
              Value = "STATE"
          }
      }
  }).Result;
```

### Open the Door
```C#
var api = new IotApi()
                .UseXmlRpc("CCU uri");
api.Open();

var result = api.SendAsync(new MethodCall()
  {
      MethodName = "setValue",
      SendParams = new System.Collections.Generic.List<Param>() {
          new Param()
          {
              Value = "LEQ1335713:1"
          },
          new Param()
          {
              Value = "STATE"
          },
          new Param()
          {
              Value = true
          }
      }
  }).Result;    

```

### Close the Door
```C#
var api = new IotApi()
                .UseXmlRpc("CCU uri");
api.Open();

 var result = api.SendAsync(new MethodCall()
   {
       MethodName = "setValue",
       SendParams = new System.Collections.Generic.List<Param>() {
           new Param()
           {
               Value = "LEQ1335713:1"
           },
           new Param()
           {
               Value = "STATE"
           },
           new Param()
           {
               Value = false
           }
       }
   }).Result;
```
You can see more test and application with XmlRpc [here](https://github.com/UniversityOfAppliedSciencesFrankfurt/SmartWorld/tree/netcore-dev/IotApi/tests/XmlRpcConnectorTests)