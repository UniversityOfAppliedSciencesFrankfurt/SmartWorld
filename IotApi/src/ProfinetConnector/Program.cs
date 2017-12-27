using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dacs7;
using Dacs7.Domain;

namespace ProfinetConnector
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var connectionString = "Data Source=127.0.0.1:102,0,2";

            //create an instance of the client
            var client = new Dacs7Client();

            //connect to the plc. If the connection could not be established
            //you will get an exception here.
            Console.WriteLine("Profinet Machine is not Connected ");
            client.Connect(connectionString);

            //Check if the client is connected. If yes, than close the connection whit a call 
            //of disconnect.
            if (client.IsConnected)
            {


                Console.WriteLine("Profinet Machine is Connected ");
                var length = 1;
                var testData = true;
                var offset = 1; //For bitoperations we need to specify the offset in bits  (byteoffset * 8 + bitnumber)
                var dbNumber = 1;

                //Write a bit to the PLC.
                client.WriteAny(PlcArea.DB, offset, testData, new int[] { length, dbNumber });

                //Read a bit from the PLC
                var state = client.ReadAny(PlcArea.DB, offset * 8, typeof(bool), new int[] { length, dbNumber });
                Console.WriteLine(state);
                Console.ReadKey();

            }

        }
    }

}





