namespace PhilipsHueTweetModule
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Azure.Devices.Shared;
    using Iot;
    using PhilipsHueConnector;
    using PhilipsHueConnector.Entities;
    using System.Collections;

    class Program
    {
        //Old 
        private static string oldTweetId;
        private static string m_GtwUri;
        private static string m_UserName;
        private static string m_DeviceId;

        static void Main(string[] args)
        {
            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");

            // Cert verification is not yet fully functional when using Windows OS for the container
            bool bypassCertVerification = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!bypassCertVerification) InstallCert();
            Init(connectionString, bypassCertVerification).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Add certificate in local cert store for use by client for secure connection to IoT Edge runtime
        /// </summary>
        static void InstallCert()
        {
            string certPath = Environment.GetEnvironmentVariable("EdgeModuleCACertificateFile");
            if (string.IsNullOrWhiteSpace(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing path to certificate file.");
            }
            else if (!File.Exists(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing certificate file.");
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certPath)));
            Console.WriteLine("Added Cert: " + certPath);
            store.Close();
        }


        /// <summary>
        /// Initializes the DeviceClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(string connectionString, bool bypassCertVerification = false)
        {
            Console.WriteLine("Connection String {0}", connectionString);

            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            // During dev you might want to bypass the cert verification. It is highly recommended to verify certs systematically in production
            if (bypassCertVerification)
            {
                mqttSetting.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            DeviceClient ioTHubModuleClient = DeviceClient.CreateFromConnectionString(connectionString, settings);

            //Callback for Twin desired properties update
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdate, ioTHubModuleClient);
            var twin = await ioTHubModuleClient.GetTwinAsync();
            await onDesiredPropertiesUpdate(twin.Properties.Desired, ioTHubModuleClient);

            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", PipeMessage, ioTHubModuleClient);
        }

        /// <summary>
        /// Assigning private properties and updating Device twin
        /// </summary>
        /// <param name="desiredProperties"></param>
        /// <param name="userContext"></param>
        /// <returns></returns>
        private static async Task onDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                var twin = new TwinCollection();
                var deviceClient = userContext as DeviceClient;

                getGtwUriUserNameAndDeviceId(desiredProperties);

                if (!string.IsNullOrEmpty(m_GtwUri) && !string.IsNullOrEmpty(m_UserName))
                {
                    twin["GtwUri"] = m_GtwUri;
                    twin["UserName"] = m_UserName;
                    twin["DeviceId"] = m_DeviceId;
                }

                if (twin.Count > 0)
                {
                    ///Updating reported properies 
                    await deviceClient.UpdateReportedPropertiesAsync(twin).ConfigureAwait(false);
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Getting gateway uri, Username and Device Id from Twin
        /// </summary>
        /// <param name="desiredProperties"></param>
        private static void getGtwUriUserNameAndDeviceId(TwinCollection desiredProperties)
        {
            Console.WriteLine("Initializing desired properties");

            if (desiredProperties.Contains("GtwUri"))
            {
                m_GtwUri = getValueFromDesiredProperties("GtwUri", desiredProperties);

            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: GtwUri");
            }

            if (desiredProperties.Contains("UserName"))
            {
                m_UserName = getValueFromDesiredProperties("UserName", desiredProperties);
            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: UserName");
            }

            if (desiredProperties.Contains("DeviceId"))
            {
                m_DeviceId = getValueFromDesiredProperties("DeviceId", desiredProperties);
            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: DeviceId");
            }
        }

        /// <summary>
        /// Get specific value from TwinCollection 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="twin"></param>
        /// <returns></returns>
        private static string getValueFromDesiredProperties(string propertyName, TwinCollection twin)
        {
            var value = twin[propertyName];
            Console.WriteLine($"Property: {propertyName}, Value: {value}");
            return value;
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> PipeMessage(Message message, object userContext)
        {
            var deviceClient = userContext as DeviceClient;
            if (deviceClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            //Preparing message 
            byte[] messageBytes = message.GetBytes();
            string newTweetId = Encoding.UTF8.GetString(messageBytes);
            Console.WriteLine($"Received Time: {DateTime.UtcNow}, Tweet Id: [{newTweetId}]");

            if (!string.IsNullOrEmpty(newTweetId))
            {
                if (oldTweetId != newTweetId)
                {
                    var tweet = Interlocked.Exchange<string>(ref oldTweetId, newTweetId);

                    await SwitchOnLight();
                    Task.Delay(5000).Wait();
                    await SwitchOffLight();
                }
            }
            await deviceClient.SendEventAsync("hueOutput", new Message(messageBytes));
            
            return MessageResponse.Completed;
        }

        /// <summary>
        /// Switch On the light
        /// </summary>
        public static async Task SwitchOnLight()
        {
            await Task.Run(()=>
            {
            var iotApi = getIotApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = m_DeviceId,

                Body = new State()
                {
                    on = true,
                    bri = 120
                },

            }).Result;});
        }

        /// <summary>
        /// Switch Off the light
        /// </summary>
        public static async Task SwitchOffLight()
        {
            await Task.Run(()=>{
            var iotApi = getIotApi();

            var result = iotApi.SendAsync(new SetLightStates()
            {
                Id = m_DeviceId,

                Body = new State()
                {
                    on = false
                },

            }).Result;});
        }

        /// <summary>
        /// Get IotApi
        /// </summary>
        /// <returns></returns>
        private static IotApi getIotApi()
        {
            var api = new IotApi();
            api.UsePhilpsQueueRest(m_GtwUri, m_UserName);
            api.Open();
            return api;
        }
    }
}
