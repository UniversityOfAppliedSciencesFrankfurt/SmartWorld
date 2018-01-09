# Philips hue 

## Discover bridge IP on network
Describe how to find Philips hue bridge IP address on network
1. Make sure bridge is connected with network.
2. Use UPnP discovery app or Philips hue discover server by visiting www.meethue.com/api/nupnp or look up your DHCP table 

## Get User Name
```C#
var username = new IotApi().GenerateUserName(Bridge_Uri);
```
## Get Lights 
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

iotApi.SendAsync(new GetLights(), (result) =>
{
    List<Device> lights = result as List<Device>;
},
(err, ex) =>
{
    throw ex;
}).Wait();
```

## Switch Off the Light
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = iotApi.SendAsync(new SetLightStates()
    {
        Id = "4", //Device id connected with Bridge 
    
        Body = new State()
        {
            on = false
        },
    
    }).Result;
```

