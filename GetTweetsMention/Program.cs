namespace GetTweetsMention
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
    using LinqToTwitter;
    using System.Linq;

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
        /// Twitter Access Token
        /// </summary>
        private static string m_AccessToken;
        /// <summary>
        /// Twitter Access Token Secret 
        /// </summary>
        private static string m_AccessTokenSecret;

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

           
                var thread = new Thread(()=> PipeMessage(ioTHubModuleClient));
                thread.Start();

            Thread.Sleep(60000);
        }

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        private static void PipeMessage(object userContext)
        {
            var tweetId = getMention();
            var deviceClient = userContext as DeviceClient;
            var mgs = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject("tweetId")));

            deviceClient.SendEventAsync("output1",mgs);
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
               var reportedProperties =  getDesiredProperties(desiredProperties);

               if(reportedProperties.Count > 0)
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
            catch(Exception ex)
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

            if (desiredProperties.Contains("AccessToken"))
            {
                m_AccessToken = desiredProperties["AccessToken"];
                reportedProperties["AccessToken"] = desiredProperties["AccessToken"];
            }
            else
            {
                throw new AggregateException("Argument \"AccessToken\" not found.");
            }

            if (desiredProperties.Contains("AccessTokenSecret"))
            {
                m_AccessTokenSecret = desiredProperties["AccessTokenSecret"];
                reportedProperties["AccessTokenSecret"] = desiredProperties["AccessTokenSecret"];
            }
            else
            {
                throw new AggregateException("Argument \"AccessTokenSecret\" not found.");
            }

            return reportedProperties;
        }

        /// <summary>
        /// Get tweet mentions 
        /// </summary>
        /// <returns></returns>
        private static string getMention()
        {
            string tweetId = string.Empty;
           try
            {
                var auth = new SingleUserAuthorizer()
                {
                    CredentialStore = new SingleUserInMemoryCredentialStore()
                    {
                        ConsumerKey = m_ConsumerKey,
                        ConsumerSecret = m_ConsumerSecret,
                        AccessToken = m_AccessToken,
                        AccessTokenSecret = m_AccessTokenSecret
                    },
                };

                auth.AuthorizeAsync().Wait();
                var twitterCtx = new TwitterContext(auth);

                var tweets = (from t in twitterCtx.Status
                            where t.Type == StatusType.Mentions &&
                            t.Count == 1
                            select t).ToList();

                foreach (var tweet in tweets)
                {
                    tweetId = tweet.StatusID.ToString();
                    System.Console.WriteLine($"Mention Id: {tweet.StatusID}");
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return tweetId;
        }
    }
}
