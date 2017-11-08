using System;
using System.Net;
using System.Net.Sockets;
using Modbus.Device;

namespace MySample
{
    /// <summary>
    ///     Demonstration of NModbus
    /// </summary>
    public class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                
                ModbusTcpMasterReadInputsFromModbusSlave();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey();
        }



        /// <summary>
        ///     Modbus TCP master and slave example.
        /// </summary>
        public static void ModbusTcpMasterReadInputsFromModbusSlave()
        {
            byte slaveId = 1;
            int port = 502;
            IPAddress address = new IPAddress(new byte[] { 127, 0, 0, 1 });

            // create and start the TCP slave
            TcpListener slaveTcp = new TcpListener(address, port);
            slaveTcp.Start();
            ModbusSlave slave = ModbusTcpSlave.CreateTcp(slaveId, slaveTcp);
            var listenTask = slave.ListenAsync();

            // create the master
            TcpClient masterTcp = new TcpClient();
            masterTcp.ConnectAsync(address.ToString(), port);
            ModbusIpMaster master = ModbusIpMaster.CreateIp(masterTcp);

            ushort numInputs = 4;
            ushort startAddress = 100;

            // read five register values
            ushort[] inputs = master.ReadInputRegisters(startAddress, numInputs);
            bool[] inp = master.ReadCoils(startAddress, 10);

            for (int i = 0; i < numInputs; i++)
            {
                Console.WriteLine($"Register {(startAddress + i)}={(inputs[i])}");
            }

            // clean up
            masterTcp.Dispose();
            slaveTcp.Stop();

            //    // output
            //    // Register 100=0
            //    // Register 101=0
            //    // Register 102=0
            //    // Register 103=0
            //    // Register 104=0
            //}
        }
    }
}
