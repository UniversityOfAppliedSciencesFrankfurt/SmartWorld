using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Collections;
using Newtonsoft.Json;
using Microsoft.Data.Sqlite;
using System.Threading;
using MQTTSn.Common.Entity.Message;
using System.IO;

namespace MQTTSnBroker
{
    public class Program
    {
        private static readonly Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly List<Socket> clientSockets = new List<Socket>();
        private const int BUFFER_SIZE = 4096;
        private const int PORT = 100;
        private const int clientLimit = 2;
        private static SqliteConnection sqliteConnection;

        static void Main()
        {
            Console.Title = "Server";
            SetupServer();
            Console.ReadLine();
            CloseAllSockets();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            var sql = $"{AppDomain.CurrentDomain.BaseDirectory}Database\\data.db";
            sqliteConnection = new SqliteConnection($"Data Source=/{sql}");

            Console.WriteLine("Server setup complete");
            CreateSockets();
        }

        private static void CreateSockets()
        {
            Socket s1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            s1.Bind(new IPEndPoint(IPAddress.Loopback, 100));
            clientSockets.Add(s1);

            foreach (Socket s in clientSockets)
            {
                ParameterizedThreadStart tStart = new ParameterizedThreadStart(SocketLoop);
                Thread thrd = new System.Threading.Thread(tStart);
                thrd.Start(s);
            }
        }

        private static void SocketLoop(Object sObj)
        {
            Socket socket = (Socket)sObj;

            byte[] header = new byte[2];
            byte[] messageBody;

            socket.Listen(0);
            Socket handler = socket.Accept();
            byte[] clientId = new byte[4];
            int length = 0;


            while (true)
            {
                try
                {
                    handler.Receive(header, 2, SocketFlags.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured in thread. Closing. " + ex.Message);
                    handler = socket.Accept();
                    handler.Receive(header, 2, SocketFlags.None);
                }

                MsgTyp type = GetMsgType(header, ref length);

                messageBody = new byte[length - 2];
                try
                {
                    handler.Receive(messageBody);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception occured in thread. Closing. " + ex.Message);
                    return;
                }

                // Create the full message
                byte[] messageFull = new byte[length];
                System.Buffer.BlockCopy(header, 0, messageFull, 0, 2);
                System.Buffer.BlockCopy(messageBody, 0, messageFull, 2, length - 2);

                switch (type)
                {
                    case MsgTyp.Connect:

                        ConnackWrk cObj = ProcessConnect(messageFull, ref clientId);
                        handler.Send(cObj.Serialized, 0, cObj.connack.length, SocketFlags.None);
                        break;

                    case MsgTyp.Register:

                        RegackWrk rObj = ProcessRegister(messageFull);
                        handler.Send(rObj.Serialized, 0, rObj.regack.length, SocketFlags.None);
                        break;


                    case MsgTyp.Publish:


                        PubackWrk pObj = ProcessPublish(messageFull);
                        handler.Send(pObj.Serialized, 0, pObj.puback.length, SocketFlags.None);
                        break;

                    case MsgTyp.Subscribe:


                        SubackWrk skObj = ProcessSubscribe(messageFull, clientId);
                        handler.Send(skObj.Serialized, 0, skObj.suback.length, SocketFlags.None);
                        break;
                    case MsgTyp.Puback:

                        PubackWrk pkObj = ProcessPuback(messageFull, clientId, sqliteConnection);
                        handler.Send(pkObj.Serialized, 0, pkObj.puback.length, SocketFlags.None);
                        break;
                }
                // Send out message to be published to this client
                PublishClients(clientId, handler);

            }

        }

        private static void PublishClients(byte[] clientId, Socket handler)
        {
            List<PublishWrk> publishCL = Publisher.GetPublishbyClient(clientId, sqliteConnection);

            foreach (PublishWrk p in publishCL)
            {
                handler.Send(p.Serialized, 0, p.publish.length, SocketFlags.None);
            }

        }

        /// <summary>
        /// Close all connected client (we do not need to shutdown the server socket as its connections
        /// are already closed with the clients).
        /// </summary>
        private static void CloseAllSockets()
        {
            foreach (Socket socket in clientSockets)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Dispose();
            }

            serverSocket.Dispose();
        }


        public static MsgTyp GetMsgType(byte[] input, ref int length)
        {
            byte msgType = input[1];
            length = Convert.ToInt32(input[0]);

            switch (msgType)
            {
                case (byte)05:
                    return MsgTyp.Connack;
                case (byte)11:
                    return MsgTyp.Regack;
                case (byte)13:
                    return MsgTyp.Puback;
                case (byte)19:
                    return MsgTyp.Suback;
                case (byte)04:
                    return MsgTyp.Connect;
                case (byte)10:
                    return MsgTyp.Register;
                case (byte)12:
                    return MsgTyp.Publish;
                case (byte)18:
                    return MsgTyp.Subscribe;
                default:
                    return MsgTyp.Connect;
            }
        }

        public static ConnackWrk ProcessConnect(byte[] input, ref byte[] clientId)
        {
            ConnectWrk connect = new ConnectWrk(input); //typecasting
            clientId = connect.connect.clientId;

            if (connect.connect.flags == Flag.cleanSession)                          //TODO: if there are multiple flags, then equal to wont work 
            {
                Clients.Delete(clientId, sqliteConnection);
                Publisher.Delete(clientId, sqliteConnection);
                Subscriber.Delete(clientId, sqliteConnection);
            }
            //read the connect object and validate the clientID
            //send appropriate response
            ConnackWrk connack = new ConnackWrk();
            if (ClientAuthenticate.Validate(clientId))
            {
                Clients client = new Clients(connect.connect);
                Clients.Insert(client, sqliteConnection);
                connack.connack.returnCode = ReturnCodes.Accepted;
            }
            else
            {
                connack.connack.returnCode = ReturnCodes.Congestion;
            }

            return connack;
        }

        public static RegackWrk ProcessRegister(byte[] regInput)
        {
            RegisterWrk register = new RegisterWrk(regInput);
            //send appropriate response
            RegackWrk regack = new RegackWrk();
            regack.regack.topicId = register.register.topicId;
            regack.regack.messageId = register.register.messageId;

            if (Register.Insert(register.register, sqliteConnection) == 1)
            {
                regack.regack.ReturnCode = ReturnCodes.Accepted;
            }
            else
            {
                regack.regack.ReturnCode = ReturnCodes.Congestion;
            }

            return regack;
        }


        public static PubackWrk ProcessPublish(byte[] pubInput)
        {
            PublishWrk publish = new PublishWrk(pubInput);
            //send appropriate response
            PubackWrk puback = new PubackWrk();
            puback.puback.topicId = publish.publish.topicId;
            puback.puback.messageId = publish.publish.messageId;


            if (Publisher.Insert(publish.publish, sqliteConnection) == 1)
            {
                puback.puback.ReturnCode = ReturnCodes.Accepted;
            }
            else
            {
                puback.puback.ReturnCode = ReturnCodes.Congestion;
            }

            return puback;
        }

        public static SubackWrk ProcessSubscribe(byte[] subInput, byte[] clientId)
        {
            SubscribeWrk subscribe = new SubscribeWrk(subInput);
            //send appropriate response
            SubackWrk suback = new SubackWrk();
            suback.suback.topicId = subscribe.subscribe.topicId;
            suback.suback.messageId = subscribe.subscribe.messageId;

            if (Subscriber.Insert(subscribe.subscribe, sqliteConnection, clientId) == 1)
            {
                suback.suback.ReturnCode = ReturnCodes.Accepted;
            }
            else
            {
                suback.suback.ReturnCode = ReturnCodes.Congestion;
            }

            return suback;
        }


        public static PubackWrk ProcessPuback(byte[] input, byte[] clientId, SqliteConnection sConnect)
        {
            PubackWrk puback = new PubackWrk(input);
            if (puback.puback.ReturnCode == ReturnCodes.Accepted)
            {
                Publisher.DeleteEntry(puback.puback.topicId, clientId, sConnect);
            }

            return puback;
        }
    }
}
