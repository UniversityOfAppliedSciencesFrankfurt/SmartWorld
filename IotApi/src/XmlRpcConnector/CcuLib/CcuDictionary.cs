using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlRpcCore;

namespace CcuLib
{
    /// <summary>
    /// Stores the dictionaries of sensors, actions and values in order to map from the global name to local (CCU world) name.
    /// Also provides method to create MethodCall object
    /// </summary>
    public class CcuDictionary
    {
        private enum typeOfDictionary { sensor = 1, action = 2, value = 3 }

        private static Dictionary<string, string> sensorDictionary = new Dictionary<string, string>();

        private static Dictionary<string, string> actionDictionary = new Dictionary<string, string>();

        private static Dictionary<string, object> valueDictionary = new Dictionary<string, object>();

        static CcuDictionary()
        {
            ClearDictionary();
            CreateDeviceDictionary();
        }

        /// <summary>
        /// Add new device to dictionaries
        /// </summary>
        /// <param name="type">Target dictionary</param>
        /// <param name="name">Global name of device</param>
        /// <param name="value">Local name of device</param>
        private void AddDictionary(typeOfDictionary type, string name, object value)
        {
            switch (type)
            {
                case typeOfDictionary.action:
                    actionDictionary.Add(name, (string)value);
                    break;

                case typeOfDictionary.sensor:
                    sensorDictionary.Add(name, (string)value);
                    break;

                case typeOfDictionary.value:
                    valueDictionary.Add(name, value);
                    break;

                default: throw new InvalidOperationException();
            }

        }

        /// <summary>
        /// Clear data in all dictionaries
        /// </summary>
        private static void ClearDictionary()
        {
            sensorDictionary.Clear();
            actionDictionary.Clear();
            valueDictionary.Clear();
        }

        /// <summary>
        /// Initialize basic dictionaries
        /// </summary>
        private static void CreateDeviceDictionary()
        {

            //Sensor Related Enteries
            //KeyMatic Related Enteries
            sensorDictionary.Add("door", "LEQ1335713:1");

            //Heater and Temperature Related Entries
            sensorDictionary.Add("temp_sensor", "LEQ1000632:1");
            sensorDictionary.Add("heater", "LEQ1206324:4");

            //Dimmaktor Related Enteries
            sensorDictionary.Add("dimmer", "LEQ0578410:1");


            //KeyMatic Related Enteries
            actionDictionary.Add("status", "STATE");
            valueDictionary.Add("unlock", true);
            valueDictionary.Add("lock", false);


            //Action Related Enteries
            //Heater Related Entries
            actionDictionary.Add("mode", "CONTROL_MODE");
            valueDictionary.Add("0", 0);
            valueDictionary.Add("1", 1);
            valueDictionary.Add("2", 2);
            actionDictionary.Add("act_temp", "ACTUAL_TEMPERATURE");
            actionDictionary.Add("set_temp", "SET_TEMPERATURE");

            //Temperature Related Enteries
            actionDictionary.Add("temperature", "TEMPERATURE");
            actionDictionary.Add("humidity", "HUMIDITY");

            //Dimmaktor Related Enteries
            actionDictionary.Add("state", "LEVEL");
            valueDictionary.Add("low", 0.25);
            valueDictionary.Add("medium", 0.5);
            valueDictionary.Add("high", 0.75);
            valueDictionary.Add("max", 1.0);
            valueDictionary.Add("off", 0.0);

        }

        /// <summary>
        /// Create a new MethodCall based all given parameters
        /// </summary>
        /// <param name="sensor">Global name of sensor</param>
        /// <param name="action">Global name of action</param>
        /// <param name="value">Global name of value</param>
        /// <returns>new MethodCall object</returns>
        public MethodCall CreateMethodCall(string sensor, string action, string value)
        {
            MethodCall method = null;
            string methodName;
            List<Param> sendParams = null;
            Param paramSensor = new Param(); //local sensor
            Param paramAction = new Param(); //local action
            Param paramValue = null; // set value

            if (action == "list")
            {
                methodName = "listDevices";
            }
            else
            {
                if ((actionDictionary.Count() == 0) || (sensorDictionary.Count() == 0) || (valueDictionary.Count() == 0))
                    CreateDeviceDictionary();
                paramSensor.Value = sensorDictionary[sensor];
                paramAction.Value = actionDictionary[action];



                if ((value == null) || (value == "get"))
                {
                    methodName = "getValue";
                    sendParams = new List<Param> { paramSensor, paramAction };

                }
                else
                {
                    methodName = "setValue";
                    paramValue = new Param();

                    // check if the input value is type of number or not
                    double retNum;
                    if (Double.TryParse(Convert.ToString(value), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum))
                        paramValue.Value = retNum;                    
                    else paramValue.Value = valueDictionary[value];

                    sendParams = new List<Param> { paramSensor, paramAction, paramValue };
                }

            }
            method = new MethodCall(methodName, sendParams);
            if (method != null) return method;
            else throw new NotImplementedException();

        }

    }
}
