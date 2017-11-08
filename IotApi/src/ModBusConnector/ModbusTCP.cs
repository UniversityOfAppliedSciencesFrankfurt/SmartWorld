using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ModBusConnector
{

    /// <summary>
    /// All methods that used to establish MODBUS connection
    /// </summary>
    /// 
    public class ModbusTCP
    {
        /// <summary>
        /// Establishment of MODBUS TCP/IP Connection
        /// </summary>
        private static IPAddress _address;
        private static byte _slaveID;
        private static int _port;
        /// <summary>
        /// Define parameters which can be manipulate outside of this class
        /// </summary>
        private static ushort _numOfInputs;
        private static ushort _numOfCoils;
        private static ushort _numOfHoldingRegister;
        private static ushort _numOfInputRegister;
        private static ushort _readstartAddress;

        private static ushort _writestartAddress;
        private static ushort _writecoil_Address;
        private static bool _writecoil_Value;
        private static bool[] _writemultiplecoils_Value;
        private static ushort[] _register_Data;
        private static ushort _register_Address;
        private static ushort _register_Value;

        //public static int numOfInputs
        //{
        //    get
        //    {
        //        return _numOfInputs;
        //    }
        //    set
        //    {
        //        _numOfInputs = value;
        //    }
        //}
        //public static int startAddress
        //{
        //    get
        //    {
        //        return _startAddress;
        //    }
        //    set
        //    {
        //        _startAddress = value;
        //    }
        //}

        private static TcpListener slaveTCP = new TcpListener(_address, _port);
        private static TcpClient masterTCP = new TcpClient();

        public ModbusTCP(IPAddress address, byte slaveID, int port)
        {
            _address = address;
            _slaveID = slaveID;
            _port = port;
        }

        public static void Start_MBServer()
        {
            slaveTCP.Start();
        }

        public static void Stop_MBServer()
        {
            slaveTCP.Stop();
        }

        public static void Start_MBCLient()
        {
            masterTCP.ConnectAsync(_address.ToString(), _port);
        }

        public static void Stop_MBClient()
        {
            masterTCP.Dispose();
        }

        public static bool[] Read_Coils(ushort read_start_addr, ushort num_coils)
        {
            num_coils = _numOfCoils;
            read_start_addr = _readstartAddress;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);
            bool[] read_result = master.ReadCoils(read_start_addr, num_coils);

            Stop_MBClient();
            Stop_MBServer();
            return read_result;
        }

        public static bool[] Read_Inputs(ushort read_start_addr, ushort num_inputs)
        {
            num_inputs = _numOfInputs;
            read_start_addr = _readstartAddress;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);
            bool[] read_result = master.ReadInputs(read_start_addr, num_inputs);

            Stop_MBClient();
            Stop_MBServer();
            return read_result;
        }

        public static ushort[] Read_HoldingRegisters(ushort read_start_addr, ushort num_register)
        {
            num_register = _numOfHoldingRegister;
            read_start_addr = _readstartAddress;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);
            ushort[] read_result = master.ReadHoldingRegisters(read_start_addr, num_register);

            Stop_MBClient();
            Stop_MBServer();
            return read_result;
        }

        public static ushort[] Read_InputRegisters(ushort read_start_addr, ushort num_register)
        {
            num_register = _numOfInputRegister;
            read_start_addr = _readstartAddress;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);
            ushort[] read_result = master.ReadInputRegisters(read_start_addr, num_register);

            Stop_MBClient();
            Stop_MBServer();
            return read_result;
        }
////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static void Write_SingleCoil(ushort write_coil_addr, bool coil_value)
        {
            write_coil_addr = _writecoil_Address;
            coil_value = _writecoil_Value;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);

            master.WriteSingleCoil(_writecoil_Address, _writecoil_Value);

            Stop_MBClient();
            Stop_MBServer();
        }

        public static void Write_MultipleCoils(ushort write_start_addr, bool[] coils_value)
        {
            coils_value = _writemultiplecoils_Value;
            write_start_addr = _writestartAddress;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);

            master.WriteMultipleCoils(write_start_addr, coils_value);

            Stop_MBClient();
            Stop_MBServer();
        }

        public static void Write_SingleRegister(ushort write_register_addr, ushort register_value)
        {
            write_register_addr = _register_Address;
            register_value = _register_Value;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);

            master.WriteSingleRegister(write_register_addr, register_value);

            Stop_MBClient();
            Stop_MBServer();
        }

        public static void Write_MultipleRegisters(ushort write_register_addr, ushort[] register_data)
        {
            write_register_addr = _register_Address;
            register_data = _register_Data;
            Start_MBServer();
            Start_MBCLient();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(_slaveID, slaveTCP);
            var listener = slave.ListenAsync();

            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTCP);

            master.WriteMultipleRegisters(write_register_addr, register_data);

            Stop_MBClient();
            Stop_MBServer();
        }
    }
}
