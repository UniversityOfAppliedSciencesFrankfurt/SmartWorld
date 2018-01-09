# SmartWorld - .NET Core
Projects related to IoT, Industrie 4.0 and IoT. This repository contains a C#/.NET IotApi, which wraps up a several protocols under unified API. 
By using of various connector implementations, one can connect to different IoT devices and/or cloud services via same API.  

## Architectural Concept
Architectural concept of IotApi -

- RegisterModule: You can register your module or connector using RegisterModule, takes IInjectableModule type parameter.
- Open: You have to open connection before sending message using module/connector, Open takes Dictionary<string, object> type parameter for passing some arguments in module/connector, if module/connector requires arguments otherwise parameter is null. 
- NextSendModule: Get or set next send module in pipeline.
- SendAsync: Send message asynchronously to remote endpoint. 
- Next Module SendAsync: Sens the message asynchronously to next remote module in the pipeline endpoint.
- NextReceiveModule: Get or set next receive module in pipeline.
- ReceiveAsync: Receive the message asynchronously from remote endpoint.
- Next Module ReceiveAsync: Sens message asynchronously from next remote module in the pipeline endpoint.
- NextAcknowledgeModule: Get or set next acknowledge module in the pipeline.
- CommitAsync: Completes asynchronously the message to remote endpoint.
- AbandonAsync: Abandon asynchronously the message to remote endpoint.
- Modules/Connectors: IotApi has some predefined modules/connectors, for example PhilipsHue, Retry, XmlRpc etc.

![](https://github.com/UniversityOfAppliedSciencesFrankfurt/SmartWorld/blob/netcore-dev/Images/ArConcept.PNG "Architectural Concept")

## Develop a module
Showing step by step how to create a module  
1. Clone or create nuget package for IotApi for using in your project. 
2. Add a class in project and inherits one of the modules (ISendModule, IReceiveModule, IAcknowledgeModule),  for example, DummyModule.cs inherits ISendModule. 
```C#
 public class DummyModule : ISendModule
    {
        public ISendModule NextSendModule { get; set; }

        public void Open(Dictionary<string, object> args)
        {
        }

        /// <summary>
        /// Send message 
        /// </summary>
        /// <param name="sensorMessage">Sensor message</param>
        /// <param name="onSuccess">Success message</param>
        /// <param name="onError">Error message</param>
        /// <param name="args">Arguments</param>
        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            //Your code to send to endpoint 
           await Task.Run(()=>{
              if (!String.IsNullOrEmpty((string)sensorMessage))
                {
                    onSuccess?.Invoke(sensorMessage);
                }
                else
                {
                    onError?.Invoke(new IotApiException("Sensor message is null"));
                }
            });
        }

        /// <summary>
        /// Send list of sensor message 
        /// </summary>
        /// <param name="sensorMessages">sensor message to send</param>
        /// <param name="onSuccess">Success message </param>
        /// <param name="onError">Error message</param>
        /// <param name="args">Arguments</param>
        /// <returns></returns>
        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            List<object> onSuccList = new List<object>();
                List<IotApiException> onErrList = new List<IotApiException>();

                foreach (var msg in sensorMessages)
                {
                    await this.SendAsync(sensorMessages, (sucMgs) =>
                    {
                        onSuccList.Add(sucMgs);
                    },
                    (err) =>
                    {
                        onErrList.Add(err);
                    },
                    args);
                }

                onSuccess?.Invoke(onSuccList);

                if (onErrList != null)
                {
                    onError?.Invoke(onErrList);
                    return;
                }
        }
    }
```
## How-to guides
Here showing how to send the message to endpoint using IotApi -

```C#
IotApi api = new IotApi().
            RegisterModule(new DummyModule());

            api.Open(new System.Collections.Generic.Dictionary<string, object>());
            
            api.SendAsync(new { Prop1 = 1.2, Prop2 = ":)" },
                (result) =>
                {
                    var response = result;
                },
                (msgs, err) =>
                {
                    var ex = err;
                    var message = msgs;
                },
                null).Wait();
```