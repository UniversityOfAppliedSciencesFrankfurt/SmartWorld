using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XmlRpcCore;

namespace CcuLib
{

    /// <summary>
    /// Contains all the methods that interact with CCU.
    /// </summary>
    public class Ccu
    {
        private string m_CcuUri;
        private TimeSpan m_TimeOut;
        public MethodCall m_CcuCall;
        public MethodResponse m_CcuResponse;
        public MethodFaultResponse m_CcuFault;
        public DeviceDescription[] m_ListDevices; //Not used 
        public bool isFault = false;
        public bool isGetMethod;
        public bool isGetList;
        public bool mock;

        /// <summary>
        /// Set up Uri of CCU and duration of Time Out for sending request.
        /// </summary>
        /// <param name="uri">Uri of CCU in string</param>
        /// <param name="timeOut">duration of Time Out in string</param>
        public void SetUriAndTimeOut(string uri, TimeSpan timeOut)
        {
            m_CcuUri = uri;
            m_TimeOut = timeOut;
        }

        /// <summary>
        /// Set up Uri without duration of Time Out for sending request.
        /// </summary>
        /// <param name="uri">Uri of CCU in string</param>
        public void SetUriAndTimeOut(string uri)
        {
            m_CcuUri = uri;
        }

        /// <summary>
        /// Analyzes and seperates from local request to essential parameters (sensor, action, value) in order to create MethodCall object  
        /// </summary>
        /// <param name="msg">local request message in string as format: "[sensor] [action] [value]</param>
        public MethodCall PrepareMethodCall(object msg)
        {
            string receivedMessage = (string)msg;

            string[] parameter = receivedMessage.Split(' ');
            string sensor = parameter[0].ToLower();//public sensor 
            string action = parameter[1].ToLower();//public action
            string value = parameter[2].ToLower();//public value

            // Check if the request is Get method
            if (value == "get") isGetMethod = true;
            else isGetMethod = false;

            // Check if the request is GetListDevice method
            if (sensor == "devices") isGetList = true;
            else isGetList = false;

            try
            {
                // Transfer the local request into XML-RPC Request using the self-created dictionaries
                CcuDictionary cd = new CcuDictionary();
              return  cd.CreateMethodCall(sensor, action, value);
            }
            catch
            {
                throw new InvalidOperationException();
            }

           
        }
        
        /// <summary>
        /// Get the list of devices connected to CCU.
        /// </summary>
        /// <param name="response">MethodResponse message</param>
        /// <returns>List of connected devices</returns>
        public string GetListDevices(MethodResponse response)
        {
            string message = "NO DEVICES FOUND";

            if (response != null)
            {
                Param result = response.ReceiveParams.First();
                if (!result.Value.GetType().IsArray)
                {
                    throw new NotSupportedException();
                }
                object[] array = (object[])result.Value;
                DeviceDescription[] listDevices = new DeviceDescription[array.Length];

                int index = 0;
                foreach (XmlRpcStruct element in array)
                {
                    DeviceDescription Device = new DeviceDescription();
                    foreach (StructMember member in element.Member)
                    {
                        switch (member.Name.ToLower())
                        {
                            case "address":
                                Device.Address = (string)member.Value;
                                break;

                            case "index":
                                Device.Channel = Convert.ToInt16(member.Value);
                                break;

                            case "children":
                                int length = ((object[])member.Value).Length;
                                Device.Children = new string[length];
                                int j = 0;
                                foreach (object child in (object[])member.Value)
                                {
                                    Device.Children[j] = child.ToString();
                                    j++;
                                }
                                break;

                            case "parent":
                                Device.Parent = (string)member.Value;
                                break;

                            case "parent_type":
                                Device.ParentType = (string)member.Value;
                                break;

                            case "type":
                                Device.Type = (string)member.Value;
                                break;
                        }
                    }

                    listDevices[index] = Device;
                    index++;
                }

                string foundDevices = "";

                if (listDevices.Count() > 0)
                {
                    foreach (DeviceDescription device in listDevices)
                    {
                        if (device.IsDevice())
                        {
                            string deviceName = DeviceDescription.RealName(device.Type);
                            foundDevices += deviceName + ", ";
                        }
                    }
                    foundDevices = foundDevices.Substring(0, foundDevices.Length - 2);
                    foundDevices += ".";

                }

                if (foundDevices != "")
                {
                    message = "FOUND DEVICES: " + foundDevices;
                }

            }
            return message;
        }

    }
}
