using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;
using System.Net.Sockets;
using System.Net;
using MQTTSnClient.Entity.Message;

namespace MQTTSnClient
{
    public class MQTTSnConnector : ISendModule
    {
        public ISendModule NextSendModule{get;set;}
        static IPEndPoint ipEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 100);
        private Socket clientSocket;

        public MQTTSnConnector(string ip, int port)
        {
            ipEndpoint = new IPEndPoint(IPAddress.Parse(ip), port);
            clientSocket = new Socket(ipEndpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            ConnectToServer(port);
        }

        public void Open(Dictionary<string, object> args)
        {
            
        }

        public void ConnectToServer(int portNumber)
        {
            int attempts = 0;

            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    // Change IPAddress.Loopback to a remote IP to connect to a remote host.
                    clientSocket.Connect(IPAddress.Loopback, portNumber);
                   //  The following is to flush out the socket of any prev messages
                    if (clientSocket.Poll(5000, SelectMode.SelectRead))
                    {
                        var buffer = new byte[2048];
                        clientSocket.Receive(buffer);
                    }
                }
                catch (SocketException ex)
                {
                    throw new IotApiException(ex.Message);
                }
            }
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public async Task SendAsync(object sensorMessage, Action<object> onSuccess = null, Action<IotApiException> onError = null, Dictionary<string, object> args = null)
        {
            try
            {
                Type t = sensorMessage.GetType();
                var re = 0;

                switch (t.Name)
                {
                    case "RegisterWrk":
                        await Task.Run(() =>
                        {
                            RegisterWrk register = sensorMessage as RegisterWrk;
                            re = clientSocket.Send(register.Serialized, 0, register.register.length, SocketFlags.None);
                        });
                        break;
                    case "ConnectWrk":
                        await Task.Run(() =>
                        {
                            ConnectWrk connect = sensorMessage as ConnectWrk;
                            re = clientSocket.Send(connect.Serialized, 0, connect.connect.length, SocketFlags.None);
                        });
                        break;
                    case "PublishWrk":
                        await Task.Run(() =>
                        {
                            PublishWrk publish = sensorMessage as PublishWrk;
                            re = clientSocket.Send(publish.Serialized, 0, publish.publish.length, SocketFlags.None);
                        });
                        break;

                    case "SubscribeWrk":
                        await Task.Run(() =>
                        {
                            SubscribeWrk subscribe = sensorMessage as SubscribeWrk;
                            re = clientSocket.Send(subscribe.Serialized, 0, subscribe.subscribe.length, SocketFlags.None);
                        });
                        break;

                    default:
                        throw new Exception("Sensor message type not found: " + t.Name);
                        break;
                }

                onSuccess?.Invoke(
                MQTTSnClient.ReceiveMessage(clientSocket)
                );
                
            }
            catch (Exception ex)
            {

                onError?.Invoke(new IotApiException(ex.Message));
            }
        }
    }
}
