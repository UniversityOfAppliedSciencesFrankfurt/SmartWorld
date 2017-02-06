namespace Modbus.Device
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using IO;
    using System.Threading;
    using System.Net;
    using System.Threading.Tasks;

    /// <summary>
    ///     Modbus TCP slave device.
    /// </summary>
    public class ModbusTcpSlave : ModbusSlave
    {
        private const int TimeWaitResponse = 1000;
        private readonly object _serverLock = new object();

        private readonly ConcurrentDictionary<string, ModbusMasterTcpConnection> m_ConnectedMasters =
            new ConcurrentDictionary<string, ModbusMasterTcpConnection>();

        private Socket m_ListenerSocket;
        private Timer _timer;
        private IPEndPoint m_ListenerEndpoint;

        private ModbusTcpSlave(byte unitId, long ipAddress = 0, short port = 8081, int timeIntervalMs = 1000)
            : base(unitId, new EmptyTransport())
        {
            m_ListenerEndpoint = new IPEndPoint(new IPAddress(new byte[] { 127, 0, 0, 1 }), port);
            m_ListenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            // _timer = new Timer(new TimerCallback(OnTimer), null, 0, (int)timeIntervalMs);
        }


        public static ModbusTcpSlave Create(byte unitId, long ipAddress = 0, short port = 8081, int timeIntervalMs = 1000)
        {
            return new ModbusTcpSlave(unitId, ipAddress, port, timeIntervalMs);
        }


        /// <summary>
        ///     Gets the Modbus TCP Masters connected to this Modbus TCP Slave.
        /// </summary>
        public ReadOnlyCollection<Socket> Masters
        {
            get
            {
                return new ReadOnlyCollection<Socket>(m_ConnectedMasters.Values.Select(masters => masters.TcpClient).ToList());
            }
        }

        /// <summary>
        ///     Gets the server.
        /// </summary>
        /// <value>The server.</value>
        /// <remarks>
        ///     This property is not thread safe, it should only be consumed within a lock.
        /// </remarks>
        private Socket Server
        {
            get
            {
                if (m_ListenerSocket == null)
                {
                    throw new ObjectDisposedException("Server");
                }

                return m_ListenerSocket;
            }
        }



        /// <summary>
        ///     Start slave listening for requests.
        /// </summary>
        public override void Listen()
        {

            Debug.WriteLine("Start Modbus Tcp Server.");

            lock (_serverLock)
            {
                try
                {
                    m_ListenerSocket.Bind(m_ListenerEndpoint);
                    m_ListenerSocket.Listen(100);
                    acceptNewMasterSocket(this);
                }
                catch (ObjectDisposedException)
                {
                    // this happens when the server stops
                }
            }
        }


        private static void acceptNewMasterSocket(ModbusTcpSlave slave)
        {
            var arg = new SocketAsyncEventArgs();
            //arg.AcceptSocket = slave.m_ListenerSocket;
            arg.UserToken = slave;
            arg.Completed += onMasterSocketAccepted;

            slave.m_ListenerSocket.AcceptAsync(arg);
        }

        /// <summary>
        ///     Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing">
        ///     <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only
        ///     unmanaged resources.
        /// </param>
        /// <remarks>Dispose is thread-safe.</remarks>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // double-check locking
                if (m_ListenerSocket != null)
                {
                    lock (_serverLock)
                    {
                        if (m_ListenerSocket != null)
                        {
                            m_ListenerSocket.Shutdown(SocketShutdown.Both);
                            m_ListenerSocket = null;

                            if (_timer != null)
                            {
                                _timer.Dispose();
                                _timer = null;
                            }

                            foreach (var key in m_ConnectedMasters.Keys)
                            {
                                ModbusMasterTcpConnection connection;

                                if (m_ConnectedMasters.TryRemove(key, out connection))
                                {
                                    connection.ModbusMasterTcpConnectionClosed -= OnMasterConnectionClosedHandler;
                                    connection.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }

        private static bool isSocketConnected(Socket socket)
        {
            return socket.Connected;
            //bool poll = socket.Poll(TimeWaitResponse, SelectMode.SelectRead);
            //bool available = (socket.Available == 0);
            //return poll && available;
        }

        private static void onMasterSocketAccepted(object sender, SocketAsyncEventArgs e)
        {
            var slave = e.UserToken as ModbusTcpSlave;
            Socket connectedMasterSocket = e.AcceptSocket;

            try
            {
                try
                {
                    // use Socket async API for compact framework compat
                    //Socket socket = null;
                    lock (slave._serverLock)
                    {
                        // Checks for disposal to an otherwise unnecessary exception (which is slow and hinders debugging).
                        if (slave.m_ListenerSocket == null)
                        {
                            return;
                        }

                        // socket = slave.Server.Server.EndAccept(ar);
                    }


                    //TcpClient client = new TcpClient { Client = socket };
                    var masterConnection = new ModbusMasterTcpConnection(connectedMasterSocket, slave);
                    masterConnection.ModbusMasterTcpConnectionClosed += slave.OnMasterConnectionClosedHandler;
                    slave.m_ConnectedMasters.TryAdd(connectedMasterSocket.RemoteEndPoint.ToString(), masterConnection);

                    Debug.WriteLine("Accept completed.");
                }
                catch (IOException ex)
                {
                    // Abandon the connection attempt and continue to accepting the next connection.
                    Debug.WriteLine("Accept failed: " + ex.Message);
                }

                // Accept another client (master)
                // use Socket async API for compact framework compat
                lock (slave._serverLock)
                {
                    acceptNewMasterSocket(slave);
                    // slave.Server.Server.BeginAccept(state => AcceptCompleted(state), slave);
                }
            }
            catch (ObjectDisposedException)
            {
                // this happens when the server stops
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }
        }



        private void OnTimer(object state)
        {
            foreach (var master in m_ConnectedMasters.ToList())
            {
                if (isSocketConnected(master.Value.TcpClient) == false)
                {
                    master.Value.Dispose();
                }
            }
        }

        private void OnMasterConnectionClosedHandler(object sender, TcpConnectionEventArgs e)
        {
            ModbusMasterTcpConnection connection;

            if (!m_ConnectedMasters.TryRemove(e.EndPoint, out connection))
            {
                string msg = $"EndPoint {e.EndPoint} cannot be removed, it does not exist.";
                throw new ArgumentException(msg);
            }

            Debug.WriteLine($"Removed Master {e.EndPoint}");
        }
    }
}
