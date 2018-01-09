# Retry Module
Enable an application to handle transient failures when it tries to connect to a service or network resource, 
by transparently retrying a failed operation. This can improve the stability of the application. Helps the user to gain access to a network or service in a more efficient way.

## Send the message
Retry module expect next module so, after retry module registration you have to register another module other wise retry module will 
throw an exception. Retry module uses three Case 
  1. Retry: Performs a specified number of retries, using a specified fixed time interval between retries.
  2. Exponential: Performs a specified number of retry after every exponential number of delay time, delay = 10, retryCount = 3 than delay time will be ```10^1,10^2 and 10^3```
  3. Geometric: Performs a specified number of retry attempts and an incremental time interval between retries, delay will be number of retries*intervalTime. 

```C#
var api = new IotApi();
    api.UseRetryModule(Case.Retry, 5, 1000);
    api.UseDummyModule();
    api.Open();
api.SendAsync("my message",(succ)=>
    {
        var s = succ;
    },(obj,err)=>
    {
        var r = err;
    }).Wait();

```

```C#
var api = new IotApi();
    api.UseRetryModule(Case.Exponential, 5, 100);
    api.UseDummyModule();
    api.Open();
var result = connector.SendAsync("my message").Result;
```

```C#
var api = new IotApi();
    api.UseRetryModule(Case.Geometric, 5, 10);
    api.UseDummyModule();
    api.Open();
var result = connector.SendAsync("my message").Result;
```