# MQTTSn
MQTT stands for Message Queue Telemetry Transport, which is an open and lightweight publish/subscribe protocol designed specifically for 
machine-to-machine and mobile applications. It is optimized for communications over networks where bandwidth is at a premium or where the 
network connection could be intermittent. However, MQTT requires an underlying network, such as TCP/IP, that provides an ordered lossless 
connection capability and this is too complex for very simple, small footprint, and low-cost devices such as wireless SAs.

## How To Run Test
If you want to run test you have to follow bellow described procedure -
1. Run MQTTSnBrocker 
2. We have used SQLite database with MQTTSnBrocker (MQTTSnBrocker/Database), edit or remove data from each table.
3. Run test. for example -

###  Setup a Connection with Brocker
```C#
var port = "100".PadLeft(4, '0');
IotApi api = new IotApi()
           .UseMQTTSnClient("127.0.0.1", 100);
api.Open();
     
ConnectWrk connect = new ConnectWrk();
connect.connect.clientId = ASCIIEncoding.ASCII.GetBytes(port);
connect.connect.flags = Flag.cleanSession;

api.SendAsync(connect, (succ) =>
 {
     var result = succ;
 }, (obj, err) =>
 {
     var er = err;
 }).Wait();
```
### Register Yout Topic
```C#
IotApi api = new IotApi()
           .UseMQTTSnClient("127.0.0.1", 100);
api.Open();

byte[] topicId = ASCIIEncoding.ASCII.GetBytes("21".PadLeft(2, '0'));
string topicName = "21jk";

RegisterWrk register = new RegisterWrk();
register.register.topicId = topicId;
register.register.topicName = ASCIIEncoding.ASCII.GetBytes(topicName);
register.register.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(regId).PadLeft(2, '0'));
register.register.length = Convert.ToByte(6 + topicName.Length);

api.SendAsync(register, (succ) =>
{
    var c = succ;
    Assert.IsNotNull(c);

}, (obj, error) =>
{
    var er = error;
}).Wait();
```

### Subscribe 
```C#
IotApi api = new IotApi()
     .UseMQTTSnClient("127.0.0.1", 100);
 api.Open();

SubscribeWrk subscribe = new SubscribeWrk();
subscribe.subscribe.topicId = ASCIIEncoding.ASCII.GetBytes("66".PadLeft(2, '0'));
subscribe.subscribe.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(subID).PadLeft(2, '0'));

api.SendAsync(subscribe, (succ) =>
{
    var r = succ;
    Assert.IsNotNull(r);

}, (obj, error) =>
{
    var er = error;
}).Wait();
```
### Publish 
```C#
IotApi api = new IotApi()
    .UseMQTTSnClient("127.0.0.1", 100);
api.Open();

PublishWrk publish = new PublishWrk();
publish.publish.topicId = ASCIIEncoding.ASCII.GetBytes("33".PadLeft(2, '0')); ;
publish.publish.data = ASCIIEncoding.ASCII.GetBytes("lights off");
publish.publish.messageId = ASCIIEncoding.ASCII.GetBytes(Convert.ToString(pubId).PadLeft(2, '0'));
publish.publish.length = Convert.ToByte(7 + "Lights off".Length);

api.SendAsync(publish, (succ) =>
{
    var suc = succ;
    Assert.IsNotNull(suc);

}, (obj, error) =>
{
    var er = error;
}).Wait();
```
