namespace PhilipsHueModule
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

            Console.WriteLine("Philip Hue module client initialized.");

            var thread = new Thread(() => threadBody(ioTHubModuleClient));
                thread.Start();
        }

        private static void threadBody(DeviceClient ioTHubModuleClient)
        {
            Console.WriteLine("Initializing IotApi and send to endpoint.");

            var api = new IotApi();
            api.UsePhilpsQueueRest(m_GtwUri,m_UserName);
            api.Open();
            
            while (true)
            {
                var result = api.SendAsync(new SetLightStates()
                {
                    Id = m_DeviceId,

                    Body = new State()
                    {
                        on = true,
                        bri = new Random().Next(120, 253),
                        hue = new Random().Next(1, 65534)
                    },

                }).Result;

                Task.Delay(3000).Wait();
            }
        }

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

                    if(twin.Count > 0)
                    {
                       await deviceClient.UpdateReportedPropertiesAsync(twin).ConfigureAwait(false);
                    }
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
        }

        private static void getGtwUriUserNameAndDeviceId(TwinCollection desiredProperties)
        {
            Console.WriteLine("Initializing desired properties");

            if (desiredProperties.Contains("GtwUri"))
            {
                m_GtwUri = getValueFromDesiredProperties("GtwUri",desiredProperties);

            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: GtwUri");
            }

            if (desiredProperties.Contains("UserName"))
            {
                m_UserName = getValueFromDesiredProperties("UserName",desiredProperties);
            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: UserName");
            }

            if (desiredProperties.Contains("DeviceId"))
            {
                m_DeviceId = getValueFromDesiredProperties("DeviceId",desiredProperties);
            }
            else
            {
                throw new ArgumentException($"Error when receiving desired property: DeviceId");
            }
        }

        private static string getValueFromDesiredProperties(string propertyName, TwinCollection twin)
        {
            var value = twin[propertyName];
            Console.WriteLine($"Property: {propertyName}, Value: {value}");
            return value;
        }
    }
}
