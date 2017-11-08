using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace ModBusConnector
{
    public class ModbusParameters
    {
        private byte slaveId;
        private int port;
        private IPAddress address;

        public byte SlaveID
        {
            get
            {
                return this.slaveId;
            }
            set
            {
                this.slaveId = value;
            }
        }

        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                this.port = value;
            }
        }

        public IPAddress IPAddress
        {
            get
            {
                return this.address;
            }
            set
            {
                this.address = value;
            }
        }
    }

}
