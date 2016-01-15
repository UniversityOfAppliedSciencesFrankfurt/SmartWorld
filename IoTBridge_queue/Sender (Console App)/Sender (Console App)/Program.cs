using IotBridge;
using IotBridge.CcuXmlRpc;
using IotBridge.SbTransport;
using XmlRpcLib;
using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Sender__Console_App_
{
    class Program
    {
        public static SbQueueTransport sbTrans = new SbQueueTransport();
        public static Message msg = new Message();
        public static Dictionary<string, object> sbArgs_1 = new Dictionary<string, object>();
        public static Dictionary<string, object> sbArgs_2 = new Dictionary<string, object>();
        public static int sensor = 0;
        public static int action = 0;
        public static float value = 0;
        static void Main(string[] args)
        {
            initalizeArgs();
            bool retValue;

            while (true)
            {
                retValue = initializeUserOption();

                if (retValue != false)
                    processReceivedMessage(msg);
            }
        }

        //-----------------------------------------------------------------------------------------------------------------//

        public static bool initializeUserOption()
        {
            bool retValue = true;
            Console.WriteLine("Choose option");
            Console.WriteLine("1 - List the devices");
            Console.WriteLine("2 - For KeyMatic Sensor");
            Console.WriteLine("3 - For Temperature Sensor");
            Console.WriteLine("4 - For Heater Sensor");
            Console.WriteLine("5 - For Dimmaktor Sensor");
            Console.WriteLine("9 - To Exit");
            string text = Console.ReadLine();
            if (text == "9")
                System.Environment.Exit(0);

            retValue = handleRemoteSensor(text);
            return retValue;

        }

        //--------------------------------------------------------------------------------------------------------------------//

        public static void processReceivedMessage(Message message)
        {
            message = sbTrans.Receive(sbArgs_2);
            string text = message.GetBody<object>().ToString();
            string[] displayResult = text.Split(',');
            if (message != null)
            {
                if (displayResult.Length > 1)
                {
                    for (int i = 1; i < displayResult.Length; i++)
                    {
                        Console.WriteLine(displayResult[i]);
                    }
                }
                else
                {
                    Console.WriteLine(displayResult[0]);
                }
            }
        }

        //-------------------------------------------------------------------------------------------------------------------//

        static public void initalizeArgs()
        {
            var ConnStr = ConfigurationManager.AppSettings["ConnectionString"];
            var queueName_1 = ConfigurationManager.AppSettings["QueueName_1"];
            var queueName_2 = ConfigurationManager.AppSettings["QueueName_2"];
            sbArgs_1.Add("m_QueueName", queueName_1);
            sbArgs_1.Add("m_ConnStr", ConnStr);
            sbArgs_2.Add("m_QueueName", queueName_2);
            sbArgs_2.Add("m_ConnStr", ConnStr);
        }

        //-------------------------------------------------------------------------------------------------------------------//

        static public bool handleRemoteSensor(string sensor)
        {
            bool ret = true;// to be removed the unassigned value
            switch (sensor)
            {
                case "1":
                    ret = getListDevice();
                    break;

                case "2":
                    ret = handleDoorSensor();
                    break;

                case "3":
                    ret = handleTempSensor();
                    break;

                case "4":
                    ret = handleHeaterSensor();
                    break;

                case "5":
                    ret = handleDimmAktorSensor();
                    break;

                default:
                    Console.WriteLine("Error in Input");
                    ret = false;
                    break;
            }
            return ret;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        static public bool getListDevice()
        {
            string command = "DEVICE LIST get";
            sendCommand(command);
            return true;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        static public bool handleDoorSensor()
        {
            string command;
            bool retValue = true;

            Console.WriteLine("KeyMatic:");

        CHOICE: Console.WriteLine("Press 1 to READ the door status");
            Console.WriteLine("Press 2 to UNLOCK the door status");
            Console.WriteLine("Press 3 to LOCK the door status");
            Console.WriteLine("Press 9 to RETURN previos window");

            string userChoice = Console.ReadLine();

            switch (userChoice)
            {
                case "1":
                    //Gets the status of the door
                    command = "DOOR STATUS get";
                    break;

                case "2":
                    //Unlocks the door
                    command = "DOOR STATUS UNLOCK";
                    break;

                case "3":
                    //Locks the door
                    command = "DOOR STATUS LOCK";
                    break;

                case "9":
                    retValue = false;
                    return retValue;

                default:
                    Console.WriteLine("Invalid Choice: Please Choose again");
                    goto CHOICE;
            }

            sendCommand(command);
            return retValue;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        static public bool handleTempSensor()
        {
            string command;
            bool retValue = true;

            Console.WriteLine("Entering the Temperature Sensor Mode");
            Console.WriteLine("Enter Your Choice for Further Action");

        CHOICE: Console.WriteLine("Press 1 to Read the Temperature");
            Console.WriteLine("Press 2 to Read the Humidity");
            Console.WriteLine("Press 9 to get back to previos window");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    //Gets the temperature from the Temperature Sensor
                    command = "TEMP_SENSOR TEMPERATURE get";
                    break;

                case "2":
                    //Gets the humidity from the Temperature Sensor
                    command = "TEMP_SENSOR HUMIDITY get";
                    break;

                case "9":
                    retValue = false;
                    return retValue;

                default:
                    Console.WriteLine("Invalid Choice: Please Choose again");
                    goto CHOICE;

            }

            sendCommand(command);
            return retValue;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        static public bool handleHeaterSensor()
        {
            string command;
            bool retValue = true;

            Console.WriteLine("Entering the Heater Control Mode");
            Console.WriteLine("Enter Your Choice for Further Operations");

        CHOICE: Console.WriteLine("Press 1 to Read the Values of Heater Control");
            Console.WriteLine("Press 2 to Read the Modes(MANUAL/AUTO) of Heater Control");
            Console.WriteLine("Press 3 to Set the Modes of Heater Control");
            Console.WriteLine("Press 4 to Set the Temperature of Heater Control");
            Console.WriteLine("Press 5 to Read the Set Temperature of Heater Control");
            Console.WriteLine("Press 9 to get back to previos window");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    //Gets the temperature on the heater sensor
                    command = "HEATER ACT_TEMP get";
                    break;

                case "2":
                    //Gets the mode to which the Heater is set
                    command = "HEATER MODE get";
                    break;

                case "3":
                    //Sets the mode of the Heater;Manual/Auto
                    command = setMode();
                    break;

                case "4":
                    //Sets the temperature of the heater as per the user's choice.
                    command = setTemperature();
                    break;

                case "5":
                    //Gets the temperature value that is set on the Heater
                    command = "HEATER SET_TEMP get";
                    break;

                case "9":
                    retValue = false;
                    return retValue;

                default:
                    Console.WriteLine("Invalid Choice: Please Choose again");
                    goto CHOICE;
            }

            sendCommand(command);
            return retValue;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        static public bool handleDimmAktorSensor()
        {
            string command;
            bool retValue = true;

            Console.WriteLine("Homematic Dimmaktor Sensor : Control the Room Ambience");

        CHOICE1: Console.WriteLine("Press 1 to display the current mode of the Dimmaktor");
            Console.WriteLine("Press 2 to change to LOW Intensity mode");
            Console.WriteLine("Press 3 to change to MEDIUM Intensity mode");
            Console.WriteLine("Press 4 to change to HIGH Intensity mode");
            Console.WriteLine("Press 5 to OFF the Dimmaktor");
            Console.WriteLine("Press 9 to return to the previous window");

            string keyValue = Console.ReadLine();

            switch (keyValue)
            {
                case "1":
                    // Gets the current mode of the Dimmer
                    command = "DIMMER STATE get";
                    break;

                case "2":
                    // Sets the desired mode for the dimmer
                    command = "DIMMER STATE LOW";
                    break;

                case "3":
                    // Sets the desired mode for the dimmer
                    command = "DIMMER STATE MEDIUM";
                    break;

                case "4":
                    // Sets the desired mode for the dimmer
                    command = "DIMMER STATE HIGH";
                    break;

                case "5":
                    // Sets the desired mode for the dimmer
                    command = "DIMMER STATE OFF";
                    break;

                case "9":
                    retValue = false;
                    return retValue;

                default:
                    Console.WriteLine("Invalid Choice: Please Choose again");
                    goto CHOICE1;
            }

            sendCommand(command);
            return retValue;
        }

        //-------------------------------------------------------------------------------------------------------------------//
        //To be called for sending the message.
        static public void sendCommand(string command)
        {
            Message msg = new Message(command);
            sbTrans.Send(msg, sbArgs_1);
            //Console.WriteLine("Message SENT !!!");Uncomment it for debugging
        }

        //-------------------------------------------------------------------------------------------------------------------//

        static public string setMode()
        {
            string mode;

        CHOICE1: Console.WriteLine("Press 1 for Auto mode");
            Console.WriteLine("Press 2 for Manual mode");

            mode = Console.ReadLine();

            switch (mode)
            {
                case "1":
                    return "HEATER MODE 0";

                case "2":
                    return "HEATER MODE 1";

                default:
                    Console.WriteLine("Invalid Choice: Please Choose again");
                    goto CHOICE1;
            }

        }

        //-------------------------------------------------------------------------------------------------------------------//

        static public string setTemperature()
        {
            string retTempString = "HEATER SET_TEMP ";
            string tempToken = "4.5";

            Console.WriteLine("Enter the value of temperature to be set");
            string temp = Console.ReadLine();
            double tempValue = Convert.ToDouble(temp);

            //The Temperature that is to be set for the heater should be in the value range of 4.5 to 30.5.
            if (tempValue > 30.5)
            {
                tempToken = "30.5";
            }
            else if (tempValue < 4.5)
            {
                tempToken = "4.5";
            }
            else
            {
                tempToken = temp;
            }

            retTempString += tempToken;

            //Console.WriteLine("String after Concatination = {0}", retTempString);Uncomment it for Debugging

            return retTempString;
        }
    }
}
