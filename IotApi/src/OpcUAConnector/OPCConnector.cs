using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Iot;
using Opc.Ua.Client;
using Opc.Ua;
using System.Security.Cryptography.X509Certificates;
using Opc.Ua.Server;
using Opc.Ua.Sample;
using Opc.Ua.Configuration;
using System.Threading;




namespace OpcUAConnector
{
    public class OPCConnector
    {


        public IReceiveModule NextReceiveModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public ISendModule NextSendModule
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        //  public void Open(Dictionary<string, object> args
        //{

        public static void Open(string[] args)
        {
            //TODO: 
            //Connection part with client and server part 
            // namespace Opc.Ua.Server { };

            Console.WriteLine(".Net Core OPC UA Console Client sample");
            string endpointURL;
            if (args.Length == 0)
            {
                // use OPC UA .Net Sample server 
                endpointURL = "opc.tcp://" + Utils.GetHostName() + ":51210/UA/SampleServer";
            }
            else
            {
                endpointURL = args[0];
            }
            try
            {
                Task t = ConsoleSampleClient(endpointURL);
                t.Wait();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exit due to Exception: {0}", e.Message);
            }
        }

        public static async Task ConsoleSampleClient(string endpointURL)
        {
            Console.WriteLine("1 - Create an Application Configuration.");
            var config = new ApplicationConfiguration()
            {
                ApplicationName = "UA Core Sample Client",
                ApplicationType = ApplicationType.Client,
                ApplicationUri = "urn:" + Utils.GetHostName() + ":OPCFoundation:CoreSampleClient",
                SecurityConfiguration = new SecurityConfiguration
                {
                    ApplicationCertificate = new CertificateIdentifier
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/MachineDefault",
                        SubjectName = "UA Core Sample Client"
                    },
                    TrustedPeerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/UA Applications",
                    },
                    TrustedIssuerCertificates = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/UA Certificate Authorities",
                    },
                    RejectedCertificateStore = new CertificateTrustList
                    {
                        StoreType = "Directory",
                        StorePath = "OPC Foundation/CertificateStores/RejectedCertificates",
                    },
                    NonceLength = 32,
                    AutoAcceptUntrustedCertificates = true
                },
                TransportConfigurations = new TransportConfigurationCollection(),
                TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
                ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 }
            };

            await config.Validate(ApplicationType.Client);

            bool haveAppCertificate = config.SecurityConfiguration.ApplicationCertificate.Certificate != null;

            if (!haveAppCertificate)
            {
                Console.WriteLine("    INFO: Creating new application certificate: {0}", config.ApplicationName);

                X509Certificate2 certificate = CertificateFactory.CreateCertificate(
                    config.SecurityConfiguration.ApplicationCertificate.StoreType,
                    config.SecurityConfiguration.ApplicationCertificate.StorePath,
                    config.ApplicationUri,
                    config.ApplicationName,
                    config.SecurityConfiguration.ApplicationCertificate.SubjectName
                    );

                config.SecurityConfiguration.ApplicationCertificate.Certificate = certificate;

            }

            haveAppCertificate = config.SecurityConfiguration.ApplicationCertificate.Certificate != null;

            if (haveAppCertificate)
            {
                config.ApplicationUri = Utils.GetApplicationUriFromCertificate(config.SecurityConfiguration.ApplicationCertificate.Certificate);

                if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
                }
            }
            else
            {
                Console.WriteLine("    WARN: missing application certificate, using unsecure connection.");
            }

            Console.WriteLine("2 - Discover endpoints of {0}.", endpointURL);
            Uri endpointURI = new Uri(endpointURL);
            var endpointCollection = DiscoverEndpoints(config, endpointURI, 10);
            var selectedEndpoint = SelectUaTcpEndpoint(endpointCollection, haveAppCertificate);
            Console.WriteLine("    Selected endpoint uses: {0}",
                selectedEndpoint.SecurityPolicyUri.Substring(selectedEndpoint.SecurityPolicyUri.LastIndexOf('#') + 1));

            Console.WriteLine("3 - Create a session with OPC UA server.");
            var endpointConfiguration = EndpointConfiguration.Create(config);
            var endpoint = new ConfiguredEndpoint(selectedEndpoint.Server, endpointConfiguration);
            endpoint.Update(selectedEndpoint);
            var session = await Session.Create(config, endpoint, true, ".Net Core OPC UA Console Client", 60000, new UserIdentity(new AnonymousIdentityToken()), null);

            Console.WriteLine("4 - Browse the OPC UA server namespace.");
            ReferenceDescriptionCollection references;
            Byte[] continuationPoint;

            references = session.FetchReferences(ObjectIds.ObjectsFolder);

            session.Browse(
                null,
                null,
                ObjectIds.ObjectsFolder,
                0u,
                BrowseDirection.Forward,
                ReferenceTypeIds.HierarchicalReferences,
                true,
                (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                out continuationPoint,
                out references);

            Console.WriteLine(" DisplayName, BrowseName, NodeClass");
            foreach (var rd in references)
            {
                Console.WriteLine(" {0}, {1}, {2}", rd.DisplayName, rd.BrowseName, rd.NodeClass);
                ReferenceDescriptionCollection nextRefs;
                byte[] nextCp;
                session.Browse(
                    null,
                    null,
                    ExpandedNodeId.ToNodeId(rd.NodeId, session.NamespaceUris),
                    0u,
                    BrowseDirection.Forward,
                    ReferenceTypeIds.HierarchicalReferences,
                    true,
                    (uint)NodeClass.Variable | (uint)NodeClass.Object | (uint)NodeClass.Method,
                    out nextCp,
                    out nextRefs);

                foreach (var nextRd in nextRefs)
                {
                    Console.WriteLine("   + {0}, {1}, {2}", nextRd.DisplayName, nextRd.BrowseName, nextRd.NodeClass);
                }
            }

            Console.WriteLine("5 - Create a subscription with publishing interval of 1 second.");
            var subscription = new Subscription(session.DefaultSubscription) { PublishingInterval = 1000 };

            Console.WriteLine("6 - Add a list of items (server current time and status) to the subscription.");
            var list = new List<MonitoredItem> {
                new MonitoredItem(subscription.DefaultItem)
                {
                    DisplayName = "ServerStatusCurrentTime", StartNodeId = "i=2258"
                }
            };
            list.ForEach(i => i.Notification += OnNotification);
            subscription.AddItems(list);

            Console.WriteLine("7 - Add the subscription to the session.");
            session.AddSubscription(subscription);
            subscription.Create();

            Console.WriteLine("8 - Running...Press any key to exit...");
            Console.ReadKey(true);
        }

        private static void OnNotification(MonitoredItem item, MonitoredItemNotificationEventArgs e)
        {
            foreach (var value in item.DequeueValues())
            {
                Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp, value.StatusCode);
            }
        }

        private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
        {
            Console.WriteLine("Accepted Certificate: {0}", e.Certificate.Subject);
            e.Accept = (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted);
        }

        private static EndpointDescriptionCollection DiscoverEndpoints(ApplicationConfiguration config, Uri discoveryUrl, int timeout)
        {
            // use a short timeout.
            EndpointConfiguration configuration = EndpointConfiguration.Create(config);
            configuration.OperationTimeout = timeout;

            using (DiscoveryClient client = DiscoveryClient.Create(
                discoveryUrl,
                EndpointConfiguration.Create(config)))
            {
                try
                {
                    EndpointDescriptionCollection endpoints = client.GetEndpoints(null);
                    ReplaceLocalHostWithRemoteHost(endpoints, discoveryUrl);
                    return endpoints;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Could not fetch endpoints from url: {0}", discoveryUrl);
                    Console.WriteLine("Reason = {0}", e.Message);
                    throw e;
                }
            }
        }

        private static EndpointDescription SelectUaTcpEndpoint(EndpointDescriptionCollection endpointCollection, bool haveCert)
        {
            EndpointDescription bestEndpoint = null;
            foreach (EndpointDescription endpoint in endpointCollection)
            {
                if (endpoint.TransportProfileUri == Profiles.UaTcpTransport)
                {
                    if (bestEndpoint == null ||
                        haveCert && (endpoint.SecurityLevel > bestEndpoint.SecurityLevel) ||
                        !haveCert && (endpoint.SecurityLevel < bestEndpoint.SecurityLevel))
                    {
                        bestEndpoint = endpoint;
                    }
                }
            }
            return bestEndpoint;
        }

        private static void ReplaceLocalHostWithRemoteHost(EndpointDescriptionCollection endpoints, Uri discoveryUrl)
        {
            foreach (EndpointDescription endpoint in endpoints)
            {
                endpoint.EndpointUrl = Utils.ReplaceLocalhost(endpoint.EndpointUrl, discoveryUrl.DnsSafeHost);
                StringCollection updatedDiscoveryUrls = new StringCollection();
                foreach (string url in endpoint.Server.DiscoveryUrls)
                {
                    updatedDiscoveryUrls.Add(Utils.ReplaceLocalhost(url, discoveryUrl.DnsSafeHost));
                }
                endpoint.Server.DiscoveryUrls = updatedDiscoveryUrls;
            }
        }
















        public Task<object> ReceiveAsync(Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
            //TODO: You have client now so receive part should be here
        }

        public Task ReceiveAsync(Action<IList<object>> onSuccess, Action<IList<object>, Exception> onError = null, Dictionary<string, object> args = null)
        {
            throw new NotImplementedException();
        }

        public Task SendAsync(IList<object> sensorMessages, Action<IList<object>> onSuccess = null, Action<IList<IotApiException>> onError = null, Dictionary<string, object> args = null)
        {


            public class ApplicationMessageDlg : IApplicationMessageDlg
        {
            private string message = string.Empty;
            private bool ask = false;

            public override void Message(string text, bool ask)
            {
                this.message = text;
                this.ask = ask;
            }

            public override async Task<bool> ShowAsync()
            {
                if (ask)
                {
                    message += " (y/n, default y): ";
                    Console.Write(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
                if (ask)
                {
                    try
                    {
                        ConsoleKeyInfo result = Console.ReadKey();
                        Console.WriteLine();
                        return await Task.FromResult((result.KeyChar == 'y') || (result.KeyChar == 'Y') || (result.KeyChar == '\r'));
                    }
                    catch
                    {
                        // intentionally fall through
                    }
                }
                return await Task.FromResult(true);
            }

        }


        public class Program
        {
            public static void Main(string[] args)
            {
                MySampleServer server = new MySampleServer();
                server.Start();
            }
        }

        public class MySampleServer
        {
            SampleServer server;
            Task status;
            DateTime lastEventTime;

            public void Start()
            {

                try
                {
                    ConsoleSampleServer().Wait();
                    Console.WriteLine("Server started. Press any key to exit...");
                }
                catch (Exception ex)
                {
                    Utils.Trace("ServiceResultException:" + ex.Message);
                    Console.WriteLine("Exception: {0}", ex.Message);
                }

                try
                {
                    Console.ReadKey(true);
                }
                catch
                {
                    // wait forever if there is no console
                    Thread.Sleep(Timeout.Infinite);
                }

                if (server != null)
                {
                    Console.WriteLine("Server stopped. Waiting for exit...");

                    server.Dispose();
                    server = null;

                    status.Wait();
                }
            }
            private static void CertificateValidator_CertificateValidation(CertificateValidator validator, CertificateValidationEventArgs e)
            {
                if (e.Error.StatusCode == StatusCodes.BadCertificateUntrusted)
                {
                    e.Accept = false;
                    Console.WriteLine("Rejected Certificate: {0}", e.Certificate.Subject);
                }
            }

            private async Task ConsoleSampleServer()
            {
                ApplicationInstance.MessageDlg = new ApplicationMessageDlg();
                ApplicationInstance application = new ApplicationInstance();

                application.ApplicationName = "UA Core Sample Server";
                application.ApplicationType = ApplicationType.Server;
                application.ConfigSectionName = "Opc.Ua.SampleServer";

                // load the application configuration.
                ApplicationConfiguration config = await application.LoadApplicationConfiguration(false);

                // check the application certificate.
                bool haveAppCertificate = await application.CheckApplicationInstanceCertificate(false, 0);
                if (!haveAppCertificate)
                {
                    throw new Exception("Application instance certificate invalid!");
                }

                if (!config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
                {
                    config.CertificateValidator.CertificateValidation += new CertificateValidationEventHandler(CertificateValidator_CertificateValidation);
                }

                // start the server.
                server = new SampleServer();
                await application.Start(server);

                // start the status thread
                status = Task.Run(new Action(StatusThread));

                // print notification on session events
                server.CurrentInstance.SessionManager.SessionActivated += EventStatus;
                server.CurrentInstance.SessionManager.SessionClosing += EventStatus;
                server.CurrentInstance.SessionManager.SessionCreated += EventStatus;

            }

            private void EventStatus(Session session, SessionEventReason reason)
            {
                lastEventTime = DateTime.UtcNow;
                PrintSessionStatus(session, reason.ToString());
            }

            void PrintSessionStatus(Session session, string reason, bool lastContact = false)
            {
                lock (session.DiagnosticsLock)
                {
                    string item = String.Format("{0,9}:{1,20}:", reason, session.SessionDiagnostics.SessionName);
                    if (lastContact)
                    {
                        item += String.Format("Last Event:{0:HH:mm:ss}", session.SessionDiagnostics.ClientLastContactTime.ToLocalTime());
                    }
                    else
                    {
                        if (session.Identity != null)
                        {
                            item += String.Format(":{0,20}", session.Identity.DisplayName);
                        }
                        item += String.Format(":{0}", session.Id);
                    }
                    Console.WriteLine(item);
                }
            }

            private void StatusThread()
            {
                while (server != null)
                {
                    if (DateTime.UtcNow - lastEventTime > TimeSpan.FromMilliseconds(6000))
                    {
                        IList<Session> sessions = server.CurrentInstance.SessionManager.GetSessions();
                        for (int ii = 0; ii < sessions.Count; ii++)
                        {
                            Session session = sessions[ii];
                            PrintSessionStatus(session, "-Status-", true);
                        }
                        lastEventTime = DateTime.UtcNow;
                    }
                    Thread.Sleep(1000);
                }
            }

        }

    }

}












