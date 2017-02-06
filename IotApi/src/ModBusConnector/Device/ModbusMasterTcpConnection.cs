namespace Modbus.Device
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;

    using IO;
    using Message;

    using Unme.Common;
    using Text_Messages;

    /// <summary>
    /// Represents an incoming connection from a Modbus master. Contains the slave's logic to process the connection.
    /// </summary>
    internal class ModbusMasterTcpConnection : ModbusDevice, IDisposable
    {
        private readonly Socket m_MasterSocket;
        private readonly string _endPoint;
        private readonly Stream _stream;
        private readonly ModbusTcpSlave m_Slave;
        private readonly AsyncCallback _readHeaderCompletedCallback;
        private readonly AsyncCallback _readFrameCompletedCallback;
        private readonly AsyncCallback _writeCompletedCallback;

        private readonly byte[] m_MsgHeader = new byte[6];
        private byte[] m_MsgFrame;

        public ModbusMasterTcpConnection(Socket client, ModbusTcpSlave slave)
            : base(new ModbusIpTransport(new ModbusTcpClient(client))) //new TcpClientAdapter(null /*client*/)))
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (slave == null)
            {
                throw new ArgumentNullException(nameof(slave));
            }

            m_MasterSocket = client;
            // _endPoint = client.Client.RemoteEndPoint.ToString();
            // _stream = client.GetStream();
            m_Slave = slave;
            // _readHeaderCompletedCallback = ReadHeaderCompleted;
            // _readFrameCompletedCallback = ReadFrameCompleted;
            //_writeCompletedCallback = WriteCompleted;

            Debug.WriteLine($"Creating new Master connection at IP:{EndPoint}");
            Debug.WriteLine("Begin reading header.");

            readNewRequest(client);

            //Stream.BeginRead(msgHeader, 0, 6, _readHeaderCompletedCallback, null);
        }

        private void readNewRequest(Socket masterSocket)
        {
            var arg = new SocketAsyncEventArgs();
            arg.AcceptSocket = masterSocket;
            arg.SetBuffer(m_MsgHeader, 0, m_MsgHeader.Length);
            arg.Completed += onHeaderReceivedFromMaster;

            m_MasterSocket.ReceiveAsync(arg);
        }

        /// <summary>
        ///     Occurs when a Modbus master TCP connection is closed.
        /// </summary>
        public event EventHandler<TcpConnectionEventArgs> ModbusMasterTcpConnectionClosed;

        public string EndPoint
        {
            get { return _endPoint; }
        }

        public Stream Stream
        {
            get { return _stream; }
        }

        public Socket TcpClient
        {
            get { return m_MasterSocket; }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _stream.Dispose();
            }

            base.Dispose(disposing);
        }

        private void onHeaderReceivedFromMaster(object sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine("Read header completed.");

            try
            {
                // this is the normal way a master closes its connection
                if (e.BytesTransferred == 0)
                {
                    Debug.WriteLine("0 bytes read, Master has closed Socket connection.");
                    this.ModbusMasterTcpConnectionClosed?.Invoke(this, new TcpConnectionEventArgs(this.m_MasterSocket.RemoteEndPoint.ToString()));
                    return;
                }

                Debug.WriteLine($"MBAP header: {string.Join(", ", this.m_MsgHeader)}");
                ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(this.m_MsgHeader, 4));
                Debug.WriteLine($"{frameLength} bytes in PDU.");

                this.m_MsgFrame = new byte[frameLength];
                //thisRef.Stream.BeginRead(this._messageFrame, 0, frameLength, thisRef._readFrameCompletedCallback, null);

                var arg = new SocketAsyncEventArgs();
                arg.SetBuffer(m_MsgFrame, 0, m_MsgFrame.Length);
                arg.AcceptSocket = e.AcceptSocket;
                arg.Completed += onDataReceivedFromMaster;

                e.AcceptSocket.ReceiveAsync(arg);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception processing request: [{ex.GetType().Name}] {ex.Message}");

                // This will typically result in the exception being unhandled, which will terminate the thread pool thread and
                // thereby the process, depending on the process's configuration. Such a crash would cause all connections to be
                // dropped, even if the slave were restarted.
                // Otherwise, the request is discarded and the slave awaits the next message. If the master is unable to synchronize
                // the frame, it can drop the connection.
                if (!(ex is IOException || ex is FormatException || ex is ObjectDisposedException))
                {
                    throw;
                }
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }

            //CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            //{
            //    // this is the normal way a master closes its connection
            //    if (thisRef.Stream.EndRead(asyncResult) == 0)
            //    {
            //        Debug.WriteLine("0 bytes read, Master has closed Socket connection.");
            //        thisRef.ModbusMasterTcpConnectionClosed?.Invoke(thisRef, new TcpConnectionEventArgs(thisRef.EndPoint));
            //        return;
            //    }

            //    Debug.WriteLine($"MBAP header: {string.Join(", ", thisRef.msgHeader)}");
            //    ushort frameLength = (ushort)IPAddress.HostToNetworkOrder(BitConverter.ToInt16(thisRef.msgHeader, 4));
            //    Debug.WriteLine($"{frameLength} bytes in PDU.");
            //    thisRef._messageFrame = new byte[frameLength];
            //    thisRef.Stream.BeginRead(thisRef._messageFrame, 0, frameLength, thisRef._readFrameCompletedCallback, null);
            //}, EndPoint);
        }

        private void onDataReceivedFromMaster(object sender, SocketAsyncEventArgs e)
        {
            //CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            //{

            try
            {
                Debug.WriteLine($"Read Frame completed {e.BytesTransferred} bytes");
                byte[] frame = this.m_MsgHeader.Concat(this.m_MsgFrame).ToArray();
                Debug.WriteLine($"RX: {string.Join(", ", frame)}");

                IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(frame.Slice(6, frame.Length - 6).ToArray());
                request.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 0));

                // perform action and build response
                IModbusMessage response = this.m_Slave.ApplyRequest(request);
                response.TransactionId = request.TransactionId;

                // write response
                byte[] responseFrame = this.Transport.BuildMessageFrame(response);
                Debug.WriteLine($"TX: {string.Join(", ", responseFrame)}");
                //thisRef.Stream.BeginWrite(responseFrame, 0, responseFrame.Length, thisRef._writeCompletedCallback, null);
                var args = new SocketAsyncEventArgs();
                args.SetBuffer(responseFrame, 0, responseFrame.Length);
                args.AcceptSocket = e.AcceptSocket;
                args.Completed += onResponseSentToMaster;
                e.AcceptSocket.SendAsync(args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception processing request: [{ex.GetType().Name}] {ex.Message}");

                // This will typically result in the exception being unhandled, which will terminate the thread pool thread and
                // thereby the process, depending on the process's configuration. Such a crash would cause all connections to be
                // dropped, even if the slave were restarted.
                // Otherwise, the request is discarded and the slave awaits the next message. If the master is unable to synchronize
                // the frame, it can drop the connection.
                if (!(ex is IOException || ex is FormatException || ex is ObjectDisposedException))
                {
                    throw;
                }
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }
            //Debug.WriteLine($"Read Frame completed {thisRef.Stream.EndRead(asyncResult)} bytes");
            //    byte[] frame = thisRef._mbapHeader.Concat(thisRef._messageFrame).ToArray();
            //    Debug.WriteLine($"RX: {string.Join(", ", frame)}");

            //    IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(frame.Slice(6, frame.Length - 6).ToArray());
            //    request.TransactionId = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 0));

            //    // perform action and build response
            //    IModbusMessage response = thisRef._slave.ApplyRequest(request);
            //    response.TransactionId = request.TransactionId;

            //    // write response
            //    byte[] responseFrame = thisRef.Transport.BuildMessageFrame(response);
            //    Debug.WriteLine($"TX: {string.Join(", ", responseFrame)}");
            //    thisRef.Stream.BeginWrite(responseFrame, 0, responseFrame.Length, thisRef._writeCompletedCallback, null);
            // }, EndPoint);
        }

        private void onResponseSentToMaster(object sender, SocketAsyncEventArgs e)
        {
            Debug.WriteLine("End write.");

            try
            {
                //thisRef.Stream.EndWrite(asyncResult);
                Debug.WriteLine("Begin reading another request.");

                readNewRequest(e.AcceptSocket);

                //thisRef.Stream.BeginRead(thisRef.m_MsgHeader, 0, 6, thisRef._readHeaderCompletedCallback, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception processing request: [{ex.GetType().Name}] {ex.Message}");

                // This will typically result in the exception being unhandled, which will terminate the thread pool thread and
                // thereby the process, depending on the process's configuration. Such a crash would cause all connections to be
                // dropped, even if the slave were restarted.
                // Otherwise, the request is discarded and the slave awaits the next message. If the master is unable to synchronize
                // the frame, it can drop the connection.
                if (!(ex is IOException || ex is FormatException || ex is ObjectDisposedException))
                {
                    throw;
                }
            }
            finally
            {
                if (e != null)
                    e.Dispose();
            }

            //CatchExceptionAndRemoveMasterEndPoint(ar, (thisRef, asyncResult) =>
            //{
            //    thisRef.Stream.EndWrite(asyncResult);
            //    Debug.WriteLine("Begin reading another request.");
            //    thisRef.Stream.BeginRead(thisRef.m_MsgHeader, 0, 6, thisRef._readHeaderCompletedCallback, null);
            //}, EndPoint);
        }

        /// <summary>
        ///     Catches all exceptions thrown when action is executed and removes the master end point.
        ///     The exception is ignored when it simply signals a master closing its connection.
        /// </summary>
        private void CatchExceptionAndRemoveMasterEndPoint(
            IAsyncResult ar,
            Action<ModbusMasterTcpConnection, IAsyncResult> action,
            string endPoint)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (endPoint == null)
            {
                throw new ArgumentNullException(nameof(endPoint));
            }

            if (endPoint == string.Empty)
            {
                throw new ArgumentException(Resources.EmptyEndPoint);
            }

            try
            {
                action(this, ar);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Exception processing request: [{ex.GetType().Name}] {ex.Message}");

                // This will typically result in the exception being unhandled, which will terminate the thread pool thread and
                // thereby the process, depending on the process's configuration. Such a crash would cause all connections to be
                // dropped, even if the slave were restarted.
                // Otherwise, the request is discarded and the slave awaits the next message. If the master is unable to synchronize
                // the frame, it can drop the connection.
                if (!(ex is IOException || ex is FormatException || ex is ObjectDisposedException))
                {
                    throw;
                }
            }
        }
    }
}
