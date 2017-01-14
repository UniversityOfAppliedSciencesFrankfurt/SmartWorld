using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcuLib
{

    /// <summary>
    /// Provides information of supported devices from CCU.
    /// </summary>
    public class DeviceDescription
    {

        public static Dictionary<string, string> DeviceTypeDitionary = new Dictionary<string, string>();

        public DeviceDescription()
        {
            Initialize();
        }

        /// <summary>
        /// Initializes a dictionary which contains technical and global names of known devices
        /// </summary>
        private static void Initialize()
        {
            if (DeviceTypeDitionary.Count() == 0)
            {
                DeviceTypeDitionary.Add("HM-RCV-50", "Central Control Unit");
                DeviceTypeDitionary.Add("HM-LC-Dim1PWM-CV", "1 Channel Dimmer PWM");
                DeviceTypeDitionary.Add("HM-WDS10-TH-O", "Temperature and Humidity Sensor");
                DeviceTypeDitionary.Add("HM-CC-RT-DN", "Wireless Radiator Thermostat (Heating Control)");
                DeviceTypeDitionary.Add("HM-Sec-Key-S", "Keymatic");
                DeviceTypeDitionary.Add("HM-RC-Key4-2", "Remote 4-2");
                DeviceTypeDitionary.Add("HM-Sec-SCo", "Optical Shutter Contact (Contact Sensor)");
            }
        }

        /// <summary>
        /// Converts the technical (local) name to specific device's name
        /// </summary>
        /// <param name="technicalName">local name of device</param>
        /// <returns>absolute name of device</returns>
        public static string RealName(string technicalName)
        {
            if (DeviceTypeDitionary.Count == 0) Initialize();
            string realName = null;
            realName = DeviceTypeDitionary[technicalName];

            return realName;
        }

        public string Type { get; set; }

        private DeviceFamily deviceFamily;

        public DeviceFamily Family
        {
            get
            {
                return deviceFamily;
            }
            set
            {
                string[] s = this.Type.Split('-');
                switch (s[0])
                {
                    case "HM":
                        deviceFamily = DeviceFamily.HomeMaticBidCoS;
                        break;

                    case "HMW":
                        deviceFamily = DeviceFamily.HomeMaticWired;
                        break;

                    default:
                        deviceFamily = DeviceFamily.Max;
                        break;
                }
            }
        }

        public int Channel { get; set; }

        public string Address { get; set; }

        public string Parent { get; set; }

        public string ParentType { get; set; }

        public string[] Children { get; set; }

        public bool IsDevice()
        {
            return string.IsNullOrEmpty(this.Parent);
        }
    }
}
