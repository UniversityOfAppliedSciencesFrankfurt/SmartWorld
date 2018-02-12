using Iot;
using System;
using System.Collections.Generic;
//using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XmlRpcCore;

namespace Test.Desktop
{
    class Program
    {

        static void Main(string[] args)
        {
            IotApi iotApi = new IotApi()
                .UseXmlRpc("http://192.168.204.99:2001");
            
            iotApi.Open();

            Console.WriteLine("Welcome to IOT Bridge Gateway!");
            Thread.Sleep(2000); // Wait two seconds
            Console.Clear();

            Functions(iotApi).Wait();

        }


        static private int RecursiveChoice(int minVal, int maxVal)
        {
            int input = Convert.ToInt32(Console.ReadLine());
            if (!(minVal <= input && input <= maxVal))
            {
                Console.WriteLine("Wrong input! Please enter a new option!");
                input = RecursiveChoice(minVal, maxVal);
            }
            Console.Clear();
            return input;
        }

        private static void Display(string stringToDisplay)
        {
            Console.WriteLine(stringToDisplay);
        }

        private static async Task Functions(IotApi iotApi)
        {
            // Function
            int funcChoice;
            Console.WriteLine("Please choose the function you want to control by pressing option's number: ");
            Console.WriteLine("1. List of connected devices");
            Console.WriteLine("2. Control Sensors");

            funcChoice = RecursiveChoice(1, 2); //change

            switch (funcChoice)
            {
                case 1:
                    string text = await Command.GetListDevices(iotApi);
                    Console.WriteLine("Devices connected to CCU: ");
                    Console.WriteLine(text);
                    //GetListDevices(m_Ccu);
                    break;
                case 2:
                    Console.Clear();
                    await Sensors(iotApi);
                    break;
            }

            Console.WriteLine("Do you want to continue?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            switch (RecursiveChoice(1, 2))
            {
                case 1:
                    await Functions(iotApi);
                    break;
                case 2:
                    Environment.Exit(2);
                    break;
            }
        }

        private static async Task Sensors(IotApi ccu)
        {
            int sensorChoice;
            Console.WriteLine("Choose the sensor you want to control: ");
            Console.WriteLine("1. Door");
            Console.WriteLine("2. Weather");
            Console.WriteLine("3. Heater");
            Console.WriteLine("4. Dimmer");
            Console.WriteLine("5. Back to Main Menu");

            sensorChoice = RecursiveChoice(1, 5);

            switch (sensorChoice)
            {
                case 1:
                    await Doors(ccu);
                    break;
                case 2:
                    await Weather(ccu);
                    break;
                case 3:
                    await Heater(ccu);
                    break;
                case 4:
                    await Dimmer(ccu);
                    break;
                case 5:
                    await Functions(ccu);
                    break;
            }
        }

        private static async Task Doors(IotApi iotApi)
        {
            // Door
            int commandChoice;
            Console.WriteLine("Choose command to control the Door");
            Console.WriteLine("1. Get door status");
            Console.WriteLine("2. Open door");
            Console.WriteLine("3. Close door");
            Console.WriteLine("4. Back to Sensors selection");

            commandChoice = RecursiveChoice(1, 4);

            string text = "";
            switch (commandChoice)
            {
                case 1:
                    text = await Command.GetDoorStatus(iotApi);
                    Console.WriteLine("Status of door: " + text);
                    break;
                case 2:
                    text = await Command.OpenDoor(iotApi);
                    Console.WriteLine(text);
                    break;
                case 3:
                    text = await Command.CloseDoor(iotApi);
                    Console.WriteLine(text);
                    break;
                case 4:
                    await Sensors(iotApi);
                    break;
            }

            Console.WriteLine("Do you want to continue?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            switch (RecursiveChoice(1, 2))
            {
                case 1:
                    await Functions(iotApi);
                    break;
                case 2:
                    Environment.Exit(2);
                    break;
            }
        }

        private static async Task Weather(IotApi iotApi)
        {
            // Weather
            int commandChoice;
            Console.WriteLine("Choose command to control the Weather sensor");
            Console.WriteLine("1. Get temperature");
            Console.WriteLine("2. Get Humidity");
            Console.WriteLine("3. Back to Sensors selection");

            commandChoice = RecursiveChoice(1, 3);

            string text = "";
            switch (commandChoice)
            {
                case 1:
                    text = await Command.GetTempSensor(iotApi);
                    Console.WriteLine("Temperature: " + text);
                    break;
                case 2:
                    text = await Command.GetHumiditySensor(iotApi);
                    Console.WriteLine("Humidity: " + text);
                    break;
                case 3:
                    await Sensors(iotApi);
                    break;
            }

            Console.WriteLine("Do you want to continue?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            switch (RecursiveChoice(1, 2))
            {
                case 1:
                    await Functions(iotApi);
                    break;
                case 2:
                    Environment.Exit(2);
                    break;
            }
        }

        private static async Task Heater(IotApi iotApi)
        {
            // Heater
            int commandChoice;
            Console.WriteLine("Choose command to control the Heater");
            Console.WriteLine("1. Get Heater's mode");
            Console.WriteLine("2. Get Heater's temperature");
            Console.WriteLine("3. Set Heater's mode");
            Console.WriteLine("4. Set Heater's temperature");
            Console.WriteLine("5. Back to Sensors selection");

            commandChoice = RecursiveChoice(1, 5);

            string text = "";
            switch (commandChoice)
            {
                case 1:
                    text = await Command.GetHeaterMode(iotApi);
                    Console.WriteLine("Heater's mode: " + text);
                    break;
                case 2:
                    text = await Command.GetHeaterTemp(iotApi);
                    Console.WriteLine("Heater's Temperature: " + text);
                    break;
                case 3:
                    Console.WriteLine("Choose Heater's Mode: ");
                    Console.WriteLine("1. MODE 0");
                    Console.WriteLine("2. MODE 1");
                    Console.WriteLine("3. MODE 2");
                    Console.WriteLine("4. Back to Heater Control");
                    int modeChoice = RecursiveChoice(1, 4);
                    if (modeChoice == 6) Dimmer(iotApi);
                    else text = await Command.SetHeaterMode(iotApi, modeChoice);
                    Console.WriteLine(text);
                    break;
                case 4:
                    Console.WriteLine("Type in temperature you want to set for the heater");
                    Console.WriteLine("Note that the temperature is from 3.5 to 30.5 Celcius degrees");
                    double outTemp = Convert.ToDouble(Console.ReadLine());
                    text = await Command.SetHeaterTemp(iotApi, outTemp);
                    Console.WriteLine(text);
                    break;
                case 5:
                    await Sensors(iotApi);
                    break;
            }

            Console.WriteLine("Do you want to continue?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            switch (RecursiveChoice(1, 2))
            {
                case 1:
                    await Functions(iotApi);
                    break;
                case 2:
                    Environment.Exit(2);
                    break;
            }
        }

        private static async Task Dimmer(IotApi iotApi)
        {
            // Dimmer
            int commandChoice;
            Console.WriteLine("Choose command to control the Dimmer");
            Console.WriteLine("1. Get Dimmer's mode");
            Console.WriteLine("2. Set Dimmer's mode");
            Console.WriteLine("3. Back to Sensors selection");

            Console.WriteLine("Your choice: ");
            commandChoice = RecursiveChoice(1, 3);

            string text = "";
            switch (commandChoice)
            {
                case 1:
                    text = await Command.GetDimmerMode(iotApi);
                    Console.WriteLine("Dimmer's mode: " + text);
                    break;
                case 2:
                    Console.WriteLine("Choose Dimmer's mode");
                    Console.WriteLine("1. OFF");
                    Console.WriteLine("2. LOW");
                    Console.WriteLine("3. MEDIUM");
                    Console.WriteLine("4. HIGH");
                    Console.WriteLine("5. MAXIMUM");
                    Console.WriteLine("6. Back to Dimmer Control");
                    int modeChoice = RecursiveChoice(1, 6);
                    if (modeChoice == 6) await Dimmer(iotApi);
                    else text = await Command.SetHeaterMode(iotApi, modeChoice);
                    Console.WriteLine(text);
                    break;
                case 3:
                    await Sensors(iotApi);
                    break;
            }

            Console.WriteLine("Do you want to continue?");
            Console.WriteLine("1. Yes");
            Console.WriteLine("2. No");

            switch (RecursiveChoice(1, 2))
            {
                case 1:
                    await Functions(iotApi);
                    break;
                case 2:
                    Environment.Exit(2);
                    break;
            }
        }
    }
}
