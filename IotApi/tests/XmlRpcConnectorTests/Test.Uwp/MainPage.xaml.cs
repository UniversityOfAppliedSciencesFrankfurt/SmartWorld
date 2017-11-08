using CcuLib;
using Iot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Test.Uwp;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using XmlRpcCore;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IOTBridge_GIT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IotApi m_Ccu = new IotApi();
        int tagHeaterMode;
        int tagDimmerMode;
        public MainPage()
        {
            this.InitializeComponent();
            Settings sett = new Settings(); //default setting
            m_Ccu.RegisterModule(new XmlRpc());

            Dictionary<string, object> agr = new Dictionary<string, object>()
            {
                { "Uri", "http://192.168.0.222:2001" }
            };

            m_Ccu.Open(agr);
        }

        /// <summary>
        /// Format the string input as a XML-RPC Request and return the Response in string. If the operation has fault, it returns an exception message.
        /// </summary>
        /// <param name="ccu">Carrier Object</param>
        /// <param name="m_Request">Local string Request. Input format: "Sensor Action Value"</param>
        /// <param name="m_Result">Response message from server in string</param>
        /// <returns></returns>
        private async Task<string> SendandReceive(IotApi iotApi, string request)
        {
            Ccu ccu = new Ccu();

            var methodCall = ccu.PrepareMethodCall(request);
            string response = "";

            await iotApi.SendAsync(methodCall, (responseMessages) =>
            {
                if (MethodResponse.isMethodResponse(responseMessages))
                {
                    MethodResponse res = responseMessages as MethodResponse;

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
             (error) =>
             {
                 response = error.Message;
             });

            return response;
        }

        /// <summary>
        /// Returns list of connected devices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetListDevices_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "DEVICES LIST get";
            string m_Result =  await SendandReceive(m_Ccu, m_Request);

            ListDevices.Text = m_Result;
        }

        /// <summary>
        /// Returns status of door (opened or closed)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetDoorStatus_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "DOOR STATUS get";
            string m_State = "";
            string m_Result = await SendandReceive(m_Ccu, m_Request);
            
            if (m_Result.ToLower() == "true") m_State = "Opened";
            if (m_Result.ToLower() == "false") m_State = "Closed";
            
            DoorStatusValue.Text = m_State;

        }

        /// <summary>
        /// Controls "OPEN" the door
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OpenDoor_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "DOOR STATUS UNLOCK";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            DoorStatusValue.Text = m_Result;

            //if (DoorStatusValue.Text == "Closed") DoorStatusValue.Text = "Opened";
        }

        /// <summary>
        /// Controls "CLOSE" the door
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CloseDoor_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "DOOR STATUS LOCK";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            DoorStatusValue.Text = m_Result;

            //if (DoorStatusValue.Text == "Opened") DoorStatusValue.Text = "Closed";
        }

        /// <summary>
        /// Returns values of temperature from temperature sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetTempSensor_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "TEMP_SENSOR TEMPERATURE get";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            WeatherResponse.Text = m_Result;
        }

        /// <summary>
        /// Returns value of Humidity from Humidity sensor
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetHumiditySensor_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "TEMP_SENSOR HUMIDITY get";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            WeatherResponse.Text = m_Result;
        }

        /// <summary>
        /// Returns mode of Heater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetMode_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "HEATER MODE get";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            HeaterResponse.Text = m_Result;
        }

        /// <summary>
        /// Returns temperature value of Heater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetTempHeater_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "HEATER ACT_TEMP get";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            HeaterResponse.Text = m_Result;
        }

        /// <summary>
        /// Check chosen heater mode from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var heaterMode = sender as RadioButton;
            this.tagHeaterMode = Convert.ToInt16(heaterMode.Tag);
        }

        /// <summary>
        /// Set mode of Heater from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetHeaterMode_Click(object sender, RoutedEventArgs e)
        {
            string mode;
            switch (tagHeaterMode)
            {
                case 0:
                    mode = "0";
                    break;
                case 1:
                    mode = "1";
                    break;

                case 2:
                    mode = "2";
                    break;
                default:
                    mode = null;
                    break;    
            }

            string m_Request = "HEATER MODE " + mode;
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            HeaterResponse.Text = m_Result;
        }

        /// <summary>
        /// Set temperature value of Heater
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetHeaterTemp_Click(object sender, RoutedEventArgs e)
        {
            string tempValue;
            int tempInt = Int32.Parse(HeaterTempValue.Text);
            if (tempInt <= 3.5) HeaterTempValue.Text = "3.5";
            else if (tempInt >= 30.5) HeaterTempValue.Text = "30.5";

            tempValue = HeaterTempValue.Text;
            string m_Request = "HEATER SET_TEMP " + tempValue;
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            HeaterResponse.Text = m_Result;
        }
        
        /// <summary>
        /// Check chosen mode of Dimmer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            var dimmerMode = sender as RadioButton;
            this.tagDimmerMode = Convert.ToInt16(dimmerMode.Tag);
        }
        
        /// <summary>
        /// Set mode of Dimmer from user
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void SetDimmer_Click(object sender, RoutedEventArgs e)
        {
            string mode;
            switch (tagDimmerMode)
            {
                case 0:
                    mode = "OFF";
                    break;

                case 1:
                    mode = "LOW";
                    break;

                case 2:
                    mode = "MEDIUM";
                    break;

                case 3:
                    mode = "HIGH";
                    break;

                case 4:
                    mode = "MAX";
                    break;

                default:
                    mode = null;
                    break;
            }

            string m_Request = "DIMMER STATE " + mode;
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            DimmerResponse.Text = m_Result;
        }

        /// <summary>
        /// Returns state of Dimmer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void GetDimmer_Click(object sender, RoutedEventArgs e)
        {
            string m_Request = "DIMMER STATE get";
            string m_Result = await SendandReceive(m_Ccu, m_Request);

            switch (m_Result)
            {
                case "0":
                    m_Result = "OFF";
                    break;

                case "0.25":
                    m_Result = "LOW";
                    break;

                case "0.5":
                    m_Result = "MEDIUM";
                    break;

                case "0.75":
                    m_Result = "HIGH";
                    break;

                case "1":
                    m_Result = "MAX";
                    break;

                default:
                    break;
            }
                 
            DimmerResponse.Text = m_Result;
        }
        
        private void HeaterTempValue_GotFocus(object sender, RoutedEventArgs e)
        {
            HeaterTempValue.Text = "";  
        }

        private void HeaterTempValue_LostFocus(object sender, RoutedEventArgs e)
        {
            if (HeaterTempValue.Text == "")
                HeaterTempValue.Text = "Temperature should be in range of 3.5 to 30.5";
        }
    }
}
