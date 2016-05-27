using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;



namespace XmlRpcLib
{

    public class CCU
    {
        //Added for the Listing the Device List
        public string[] typeCompare = new string[]
        {   
                "WEATHER",
                "WEATHER_RECEIVER",
                "CLIMATECONTROL_RECEIVER",
                "WINDOW_SWITCH_RECEIVER",
                "CLIMATECONTROL_RT_TRANSCEIVER",
                "CLIMATECONTROL_RT_RECEIVER",
                "REMOTECONTROL_RECEIVER",
                "KEYMATIC",
                "SHUTTER_CONTACT",
                "DIMMER"
        };

        static XmlRpcLib.DeviceDesription.DeviceDescription[] devices;
        static string GlobalUrl;
        static int globalTimeOut;

        public Dictionary<string, string> sensorDictionary = new Dictionary<string, string>();

        public Dictionary<string, string> actionDictionary = new Dictionary<string, string>();

        public void Initialize(string Url, int timeOut)
        {
            CreateDeviceDictionary();
            GlobalUrl = Url;
            globalTimeOut = timeOut;
        }

        public string DeviceList()
        {
            //Creating the interface for fectching the device
            IXmlRpcAll clientProxy;
            clientProxy = XmlRpcProxyGen.Create<IXmlRpcAll>();
            clientProxy.Url = GlobalUrl;
            clientProxy.Timeout = globalTimeOut;

            string[] deviceList = new string[10];
            string deviceListToBePrinted = "No Devices Found";
            string token = ",";

            devices = clientProxy.ListDevices();

            int length1 = 0;
            int length2 = 0;

            // Sorting the Fetched Device List
            length1 = devices.Length;
            length2 = typeCompare.Length;


            if (length1 == 0)
            {
                return deviceListToBePrinted;
            }

            for (int i = 0; i < length1; i++)
            {
                for (int j = 0; j < length2; j++)
                {
                    if (String.Compare(devices[i].Type, typeCompare[j]) == 0)
                    {
                        deviceListToBePrinted += token;
                        deviceListToBePrinted += devices[i].Type;
                        break;
                    }
                }

            }

            return deviceListToBePrinted;
        }

        public void SetValue(string sensor, string action, string value)
        {
            string localSensor = sensorDictionary[sensor];
            string localAction = actionDictionary[action];

            switch (localSensor)
            {
                case "LEQ1335713:1":
                    IXmlRpcKeyMatic keymaticProxy;
                    keymaticProxy = XmlRpcProxyGen.Create<IXmlRpcKeyMatic>();
                    keymaticProxy.Url = GlobalUrl;
                    keymaticProxy.Timeout = globalTimeOut;

                    if (value == "UNLOCK")
                        try
                        {
                            keymaticProxy.SetValue(localSensor, localAction, true);
                        }
                        catch
                        {
                            throw;
                        }

                    if (value == "LOCK")
                        try
                        {
                            keymaticProxy.SetValue(localSensor, localAction, false);
                        }
                        catch
                        {
                            throw;
                        }

                    break;

                case "LEQ1206324:4":
                    if (localAction == "SET_TEMPERATURE")
                    {
                        IXmlRpcHeaterSensor setTempProxy;
                        setTempProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterSensor>();
                        setTempProxy.Url = GlobalUrl;
                        setTempProxy.Timeout = globalTimeOut;
                        double tValue = Convert.ToDouble(value);
                        try
                        {
                            setTempProxy.SetValue(localSensor, localAction, tValue);
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else
                    {
                        IXmlRpcHeaterSensor setModeProxy;
                        setModeProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterSensor>();
                        setModeProxy.Url = GlobalUrl;
                        int mode = Convert.ToInt32(value);
                        setModeProxy.SetValue(localSensor, localAction, mode);
                    }

                    break;
                case "LEQ0578410:1":
                    IXmlRpcDimmaktorSensor dimmaktorProxy;
                    dimmaktorProxy = XmlRpcProxyGen.Create<IXmlRpcDimmaktorSensor>();
                    dimmaktorProxy.Url = GlobalUrl;
                    dimmaktorProxy.Timeout = globalTimeOut;

                    if (value == "LOW")
                        try
                        {
                            dimmaktorProxy.SetValue(localSensor, localAction, 0.20F);
                        }
                        catch
                        {
                            throw;
                        }
                    else if (value == "MEDIUM")
                        try
                        {
                            dimmaktorProxy.SetValue(localSensor, localAction, 0.50F);
                        }
                        catch
                        {
                            throw;
                        }
                    else if (value == "HIGH")
                        try
                        {
                            dimmaktorProxy.SetValue(localSensor, localAction, 1.0F);
                        }
                        catch
                        {
                            throw;
                        }
                    else
                        try
                        {
                            dimmaktorProxy.SetValue(localSensor, localAction, 0.0F);
                        }
                        catch
                        {
                            throw;
                        }
                    break;
                default:
                    break;
            }
        }

        public string GetValue(string sensor, string action)
        {
            string retValue;
            string localSensor = sensorDictionary[sensor];
            string localAction = actionDictionary[action];

            switch (localSensor)
            {
                //Handling of KeyMatic Sensor
                case "LEQ1335713:1":
                    IXmlRpcKeyMatic keymaticProxy;
                    keymaticProxy = XmlRpcProxyGen.Create<IXmlRpcKeyMatic>();
                    keymaticProxy.Url = GlobalUrl;
                    keymaticProxy.Timeout = globalTimeOut;
                    try
                    {
                        var state = keymaticProxy.GetValue(localSensor, localAction);
                        retValue = state.ToString();
                    }
                    catch
                    {
                        throw;
                    }

                    break;

                //Handling of Temperature Sensor
                case "LEQ1000632:1":
                    if (localAction == "TEMPERATURE")
                    {
                        IXmlRpcTemperatureSensor tempProxy;
                        tempProxy = XmlRpcProxyGen.Create<IXmlRpcTemperatureSensor>();
                        tempProxy.Url = GlobalUrl;
                        tempProxy.Timeout = globalTimeOut;
                        double temperature = tempProxy.GetValue(localSensor, localAction);
                        retValue = temperature.ToString();
                    }
                    else
                    {
                        IXmlRpcHeaterHumiditySensor humidityProxy;
                        humidityProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterHumiditySensor>();
                        humidityProxy.Url = GlobalUrl;
                        humidityProxy.Timeout = globalTimeOut;
                        int humidity = humidityProxy.GetValue(localSensor, localAction);
                        retValue = humidity.ToString();
                    }
                    break;

                //Handling of Heater Sensor
                case "LEQ1206324:4":
                    if (localAction == "ACTUAL_TEMPERATURE")
                    {
                        IXmlRpcHeaterSensor actTempProxy;
                        actTempProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterSensor>();
                        actTempProxy.Url = GlobalUrl;
                        actTempProxy.Timeout = globalTimeOut;
                        double actTemp = actTempProxy.GetValue(localSensor, localAction);
                        retValue = actTemp.ToString();
                    }
                    else if (localAction == "SET_TEMPERATURE")
                    {
                        IXmlRpcHeaterSensor setTempProxy;
                        setTempProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterSensor>();
                        setTempProxy.Url = GlobalUrl;
                        double setTemp = setTempProxy.GetValue(localSensor, localAction);
                        retValue = setTemp.ToString();
                    }
                    else
                    {
                        IXmlRpcHeaterHumiditySensor getModeProxy;
                        getModeProxy = XmlRpcProxyGen.Create<IXmlRpcHeaterHumiditySensor>();
                        getModeProxy.Url = GlobalUrl;
                        getModeProxy.Timeout = globalTimeOut;
                        int mode = getModeProxy.GetValue(localSensor, localAction);
                        if (mode == 0)
                            retValue = "AUTO";
                        else if (mode == 1)
                            retValue = "MANUAL";
                        else
                            retValue = "Unknown Mode: Mode Num = " + mode.ToString();
                    }

                    break;

                //Handling of DimmAktor Sensor
                case "LEQ0578410:1":
                    IXmlRpcDimmaktorSensor dimmaktorProxy;
                    dimmaktorProxy = XmlRpcProxyGen.Create<IXmlRpcDimmaktorSensor>();
                    dimmaktorProxy.Url = GlobalUrl;
                    dimmaktorProxy.Timeout = globalTimeOut;
                    string message;
                    try
                    {
                        object level = dimmaktorProxy.GetValue(localSensor, localAction);
                        if (level.Equals(0.0))
                        {
                            message = "Dimmaktor is OFF";
                        }
                        else if (level.Equals(0.20))
                        {
                            message = "Dimmaktor is in Low Intensity mode";
                        }
                        else if (level.Equals(0.50))
                        {
                            message = "Dimmaktor is in Medium Intensity mode";
                        }
                        else if (level.Equals(1.0))
                        {
                            message = "Dimmaktor is in High Intensity mode";
                        }
                        else
                        {
                            message = "Invalid mode of Dimmaktor";
                        }
                        retValue = message;
                    }
                    catch
                    {

                        throw;
                    }
                    break;

                default:
                    retValue = "ERROR";
                    break;
            }

            return retValue;
        }

        public void CreateDeviceDictionary()
        {
            //Sensor Related Enteries
            //KeyMatic Related Enteries
            sensorDictionary.Add("DOOR", "LEQ1335713:1");

            //Heater and Temperature Related Entries
            sensorDictionary.Add("TEMP_SENSOR", "LEQ1000632:1");
            sensorDictionary.Add("HEATER", "LEQ1206324:4");

            //KeyMatic Related Enteries
            actionDictionary.Add("STATUS", "STATE");
            actionDictionary.Add("UNLOCK", "OPEN");
            actionDictionary.Add("LOCK", "CLOSED");

            //Dimmaktor Related Enteries
            sensorDictionary.Add("DIMMER", "LEQ0578410:1");


            //Action Related Enteries
            //Heater Related Entries
            actionDictionary.Add("MODE", "CONTROL_MODE");
            actionDictionary.Add("ACT_TEMP", "ACTUAL_TEMPERATURE");
            actionDictionary.Add("SET_TEMP", "SET_TEMPERATURE");

            //Temperature Related Enteries
            actionDictionary.Add("TEMPERATURE", "TEMPERATURE");
            actionDictionary.Add("HUMIDITY", "HUMIDITY");

            //Dimmaktor Related Enteries
            actionDictionary.Add("STATE", "LEVEL");
            actionDictionary.Add("LOW", "0.20F");
            actionDictionary.Add("MEDIUM", "0.50F");
            actionDictionary.Add("HIGH", "1.0F");
            actionDictionary.Add("OFF", "0.0F");

        }
    }
}
