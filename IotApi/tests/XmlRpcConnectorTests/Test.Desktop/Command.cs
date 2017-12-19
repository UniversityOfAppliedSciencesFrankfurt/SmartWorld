using CcuLib;
using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlRpcCore;


namespace Test.Desktop
{
    public class Command
    {
        private static async Task<string> RequestAndReceive(IotApi iotApi, string request)
        {
            Ccu ccu = new Ccu();

            var methodCall = ccu.PrepareMethodCall(request);
            string response = "";

            await iotApi.SendAsync(methodCall, (responseMsg) => {

                if (MethodResponse.isMethodResponse(responseMsg))
                {
                    MethodResponse res = responseMsg as MethodResponse;

                    if (ccu.isGetList)
                    {
                        response = ccu.GetListDevices(res);
                    }
                    else
                    {
                        if (!ccu.isGetMethod)
                        {
                            // Set Methods do not return any value. Detecting no value means the operation is done
                            if (res.ReceiveParams.Count() == 0) response = "Operation is done!";

                            // Set methods can not return any value
                            else throw new InvalidOperationException("The operation cannot return any value!");
                        }
                        else
                        {
                            // Get methods must return a value, if not it must be an error
                            if (res.ReceiveParams.Count() == 0) throw new InvalidOperationException("No value returned or error");

                            // Collecting the returned value
                            else response = res.ReceiveParams.First().Value.ToString();
                        }
                    }
                }
            },
            (error,ex) =>
            {
                if (error.Count > 0)
                {
                    foreach (var er in error)
                    {
                        MethodFaultResponse faultRes = er as MethodFaultResponse;
                        response = faultRes.Message;
                    }
                }
            });


            return response;
        }

        /// <summary>
        /// Get list of devices
        /// </summary>
        /// <param name="iotApi"></param>
        /// <returns></returns>
        public static async Task<string> GetListDevices(IotApi iotApi)
        {
            string request = "DEVICES LIST get";
            string result = await RequestAndReceive(iotApi, request);
            return result;
        }

        /// <summary>
        /// Get door status
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> GetDoorStatus(IotApi ccu)
        {
            string request = "DOOR STATUS get";
            string result = await RequestAndReceive(ccu, request);
            string state = "";
            if (result.ToLower() == "true") state = "Opened";
            if (result.ToLower() == "false") state = "Closed";
            return state;
        }

        /// <summary>
        /// Open the door
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> OpenDoor(IotApi ccu)
        {
            string request = "DOOR STATUS UNLOCK";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Close the door
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> CloseDoor(IotApi ccu)
        {
            string request = "DOOR STATUS LOCK";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        public static async Task<string> GetTempSensor(IotApi ccu)
        {
            string request = "TEMP_SENSOR TEMPERATURE get";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Get humidity
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> GetHumiditySensor(IotApi ccu)
        {
            string request = "TEMP_SENSOR HUMIDITY get";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Get heater mode
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> GetHeaterMode(IotApi ccu)
        {
            string request = "HEATER MODE get";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Get heater temperature 
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> GetHeaterTemp(IotApi ccu)
        {
            string request = "HEATER ACT_TEMP get";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Set heater mode
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static async Task<string> SetHeaterMode(IotApi ccu, int mode)
        {
            string request = "HEATER MODE " + mode.ToString();
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Set heater temperature 
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="tempVal"></param>
        /// <returns></returns>
        public static async Task<string> SetHeaterTemp(IotApi ccu, double tempVal)
        {
            if (tempVal <= 3.5) tempVal = 3.5;
            else if (tempVal >= 30.5) tempVal = 30.5;
            string request = "HEATER SET_TEMP " + tempVal.ToString();
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Set dimmer mode
        /// </summary>
        /// <param name="ccu"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static async Task<string> SetDimmerMode(IotApi ccu, string mode)
        {
            string request = "DIMMER STATE " + mode;
            string result = await RequestAndReceive(ccu, request);
            return result;
        }

        /// <summary>
        /// Get dimmer mode
        /// </summary>
        /// <param name="ccu"></param>
        /// <returns></returns>
        public static async Task<string> GetDimmerMode(IotApi ccu)
        {
            string request = "DIMMER STATE get";
            string result = await RequestAndReceive(ccu, request);
            return result;
        }
    }
}
