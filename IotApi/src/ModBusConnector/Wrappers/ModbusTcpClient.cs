using Modbus.IO;
using System.IO;
using System.Threading;

namespace System.Net.Sockets
{
    internal class ModbusTcpClient : IStreamResource
    {
        private IPAddress m_Address;

        private short m_Port;

        private byte[] m_Buffer;
        private Socket client;

        public ModbusTcpClient(Socket client)
        {
            this.client = client;
        }

        public ModbusTcpClient(IPAddress address, short port)
        {
            m_Port = port;
            m_Address = address;

            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var arg = new SocketAsyncEventArgs();
            arg.Completed += onConnected;
            arg.RemoteEndPoint = new IPEndPoint(address, port);
            ManualResetEvent mEvent = new ManualResetEvent(false);

            arg.UserToken = mEvent;

            Socket.ConnectAsync(arg);

            mEvent.WaitOne();

            if (arg.SocketError != SocketError.Success)
                throw new Exception(arg.SocketError.ToString());
        }



        private void onConnected(object sender, SocketAsyncEventArgs e)
        {
            ((ManualResetEvent)e.UserToken).Set();

            e.Dispose();
        }

        public Socket Socket { get; set; }

        public int InfiniteTimeout
        {
            get
            {
                return int.MaxValue;
            }
        }

        public int ReadTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public int WriteTimeout
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public void DiscardInBuffer()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += onReadCompleted;
            args.SetBuffer(buffer, offset, count);

            ManualResetEvent mEvent = new ManualResetEvent(false);
            args.UserToken = mEvent;

            Socket.ReceiveAsync(args);

            mEvent.WaitOne();

            return args.BytesTransferred;
        }

        private void onReadCompleted(object sender, SocketAsyncEventArgs e)
        {
            ManualResetEvent mEvent = e.UserToken as ManualResetEvent;

            mEvent.Set();

            e.Dispose();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            var args = new SocketAsyncEventArgs();
            args.Completed += onSendComleted;
            args.SetBuffer(buffer, offset, count);

            ManualResetEvent mEvent = new ManualResetEvent(false);

            args.UserToken = mEvent;


            Socket.SendAsync(args);

            mEvent.WaitOne();
        }

        private void onSendComleted(object sender, SocketAsyncEventArgs e)
        {
            ((ManualResetEvent)e.UserToken).Set();

            e.Dispose();
        }

        internal Stream GetStream2()
        {
            throw new NotImplementedException();
            //  this.Client.)
        }
    }
}