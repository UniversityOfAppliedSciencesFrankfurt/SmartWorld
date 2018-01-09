# Philips hue 

## Discover bridge IP on network
Describe how to find Philips hue bridge IP address on network
1. Make sure bridge is connected with network.
2. Use UPnP discovery app or Philips hue discover server by visiting www.meethue.com/api/nupnp or look up your DHCP table 

## Philips Hue Commands 
Bellow describe Philips hue commands - 

### Get User Name
```C#
var username = new IotApi().GenerateUserName(Bridge_Uri);
```
### Get Lights 
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
### Get Light State 
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = api.SendAsync(new GetLightStates()
   {
       Id = "4"
   }).Result;
```

### Switch On the Light
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = iotApi.SendAsync(new SetLightStates()
    {
        Id = "4", //Device id connected with Bridge 
    
        Body = new State()
        {
            on = true,
            bri = 120
        },
    
    }).Result;
```

### Switch Off the Light
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
### Set Light Color
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = iotApi.SendAsync(new SetLightStates()
 {
     Id = "4",

     Body = new State()
     {
         on = true,
         bri = 120,
         xy = new List<double>()
          {
             0.692, 0.308
          }
     },

 }).Result;
```
### Set Light attributes 
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = api.SendAsync(new SetLightAttributes()
  {
      Id = "1",
      Body = new
      {
          name = "Bedroom Light"
      }
  }).Result;
```
### Get new Light

```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = api.SendAsync(new GetNewLight()).Result as List<Device>;
```

### Search for new Lights
```C#
IotApi api = new IotApi();

api.UsePhilpsQueueRest(GtwUri, UsrName);

api.Open();

var result = api.SendAsync(new SerarchNewLights()
 {
     Body = new
     {
         deviceid = new []{"45AF34","543636","34AFBE" }
     }
 }).Result;
```

