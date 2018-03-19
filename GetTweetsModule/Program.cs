namespace GetTweetsModule
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
    using Newtonsoft.Json;

    class Program
    {
        /// <summary>
        /// Twiter Consumer Key
        /// </summary>
        private static string m_ConsumerKey;
        /// <summary>
        /// Twitter Consumer Secret
        /// </summary>
        private static string m_ConsumerSecret;
        /// <summary>
        /// Twitter user name
        /// </summary>
        private static string m_TwitterUserName;

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

            //Call back for desired properties update
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdate, ioTHubModuleClient);
            var twin = await ioTHubModuleClient.GetTwinAsync();
            await onDesiredPropertiesUpdate(twin.Properties.Desired, ioTHubModuleClient);

            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");

            var thread = new Thread(() => threadBody(ioTHubModuleClient));
            thread.Start();
        }

        /// <summary>
        /// Preparing message and send to IotHub
        /// </summary>
        /// <param name="userContext"></param>
        private static void threadBody(object userContext)
        {
            var deviceClient = userContext as DeviceClient;
            if (deviceClient == null)
            {
                throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
            }

            var credential = Twitter.getCredential(m_ConsumerKey, m_ConsumerSecret);
            
            while (true)
            {
                var token = Twitter.getToken(credential);
                var tweetId = Twitter.getTweetId(m_TwitterUserName, 1, token);
                var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(tweetId)));

                deviceClient.SendEventAsync("tweetOutput", message).Wait();
                Console.WriteLine("Event has been sent to tweetOutput endpoint.");

                Thread.Sleep(5000);
            }
        }

        private static async Task onDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
        {
            try
            {
                System.Console.WriteLine("Desired property change:");
                System.Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

                var deviceClient = userContext as DeviceClient;
                if (deviceClient == null)
                {
                    throw new InvalidOperationException("UserContext doesn't contain " + "expected values");
                }

                //Update device twin
                var reportedProperties = getDesiredProperties(desiredProperties);

                if (reportedProperties.Count > 0)
                {
                    await deviceClient.UpdateReportedPropertiesAsync(reportedProperties).ConfigureAwait(false);
                }

            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error when receiving desired property: {0}", exception);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="desiredProperties"></param>
        /// <returns></returns>
        private static TwinCollection getDesiredProperties(TwinCollection desiredProperties)
        {
            var reportedProperties = new TwinCollection();

            if (desiredProperties.Contains("ConsumerKey"))
            {
                m_ConsumerKey = desiredProperties["ConsumerKey"];
                reportedProperties["ConsumerKey"] = desiredProperties["ConsumerKey"];
            }
            else
            {
                throw new ArgumentException($"Argument \"ConsumerKey\" not found.");
            }

            if (desiredProperties.Contains("ConsumerSecret"))
            {
                m_ConsumerSecret = desiredProperties["ConsumerSecret"];
                reportedProperties["ConsumerSecret"] = desiredProperties["ConsumerSecret"];
            }
            else
            {
                throw new AggregateException("Argument \"ConsumerSecret\" not found.");
            }

            if (desiredProperties.Contains("UserName"))
            {
                m_TwitterUserName = desiredProperties["UserName"];
                reportedProperties["UserName"] = desiredProperties["UserName"];
            }
            else
            {
                throw new AggregateException("Argument \"UserName\" not found.");
            }

            return reportedProperties;
        }
    }
}
