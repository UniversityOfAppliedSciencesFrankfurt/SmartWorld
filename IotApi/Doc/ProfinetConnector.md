# Profinet
PROFINET IO is very similar to Profibus on Ethernet. While Profibus uses cyclic communications to exchange data with Programmable Controllers at a maximum speed of 
12Meg baud PROFINET IO uses cyclic data transfer to exchange data with Programmable Controllers over Ethernet. As with Profibus, a Programmable Controller and a 
device must both have a prior understanding of the data structure and meaning. In both systems data is organized as slots containing modules with the total number 
of I/O points for a system the sum of the I/O points for the individual modules.

## Guides
This section described how to use Profinet protocol with IotApi -

1. For sending message, you have to send ISensorMessage.

```C#
public void SendMessage()
{
    int length = 500;
    byte[] testData = new byte[500];
    int offset = 10; //The offset is the number of bytes from the beginning of the area
    int dbNumber = 560;

    var mgs = new SensorMessage() //ISensorMessage
    {
        Area = PlcArea.DB,
        Offset = offset,
        Value = testData,
        Args = new[] { length, dbNumber }

    };

    var api = new IotApi()
        .UseProfinet("Data Source=127.0.0.1:102,0,2");
    api.Open();

    api.SendAsync(mgs, (succ) =>
    {

        var succMgs = succ;

    }, (obj, err) =>
    {

        var error = err;

    }).Wait();
}
``` 
For more details about sending ISensorMessage see [tests.](https://github.com/UniversityOfAppliedSciencesFrankfurt/SmartWorld/tree/netcore-dev/IotApi/tests/ProfinetConnectorUnitTest)

2. For reading message from PLC, you have to use IReadMessage.

```C#
public void ReadMessage()
{
    var length = 1;
    var offset = 1; //For bitoperations we need to specify the offset in bits  (byteoffset * 8 + bitnumber)
    var dbNumber = 1;

    var rMgs = new Dictionary<string, object>();
    rMgs.Add("readMessage", new ReadMessage() //IReadMessage
    {
        Area = PlcArea.DB,
        Offset = offset * 8,
        Type = typeof(bool),
        Args = new int[] { length, dbNumber }
    });

    var api = new IotApi()
        .UseProfinet("Data Source=127.0.0.1:102,0,2");
    api.Open();

    var result = api.ReceiveAsync(rMgs).Result;
}
```

For more details see [tests.](https://github.com/UniversityOfAppliedSciencesFrankfurt/SmartWorld/tree/netcore-dev/IotApi/tests/ProfinetConnectorUnitTest)
