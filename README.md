# SmartWorld - .NET Core
Projects related to IoT, Industrie 4.0 and IoT. This repository contains a C#/.NET IotApi, which wraps up a several protocols under unified API. 
By using of various connector implementations, one can connect to different IoT devices and/or cloud services via same API.  

## Architectural Concept
Here describe architectural concept of IotApi

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

![alt text](https://github.com/UniversityOfAppliedSciencesFrankfurt/SmartWorld/blob/netcore-dev/Images/ArConcept.PNG, "Architectural Concept")
