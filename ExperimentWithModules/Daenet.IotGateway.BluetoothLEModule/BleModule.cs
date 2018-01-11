using Daenet.Azure.Devices.Gateway;
using Daenet.IotGateway.Common.Logger;
using Daenet.IotGateway.Devices.Gateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.Advertisement;

namespace Daenet.IotGateway.BluetoothLEModule
{
    /// <summary>
    /// Receives events from BT device.
    /// </summary>
    public class BleModule : IGatewayModule, IGatewayModuleStart
    {
        private Task m_Task;

        /// <summary>
        /// Holds list of watchers for every manufacturer in the configuration
        /// </summary>
        Dictionary<string, BluetoothLEAdvertisementWatcher> m_BleWatchers;

        private Broker m_Broker;

        private BleConfig m_Config;


        public BleModule()
        {

        }

        /// <summary>
        /// Creates a new module that starts Bluetooth watchers
        /// </summary>
        /// <param name="broker"></param>
        /// <param name="configuration"></param>
        public void Create(Broker broker, byte[] configuration)
        {
            this.m_Broker = broker;

            this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfBleModuleCreateEntered, $"Method Created() entered.");

            // this.m_BleWatcher = new BluetoothLEAdvertisementWatcher();
            this.m_BleWatchers = new Dictionary<string, BluetoothLEAdvertisementWatcher>();
            this.m_Config = JsonConvert.DeserializeObject<BleConfig>(Encoding.UTF8.GetString(configuration));

            //Validate the configuration of the Ble module
            validateConfig(this.m_Config);

            this.m_Broker.Logger.Log(LogLevel.Debug, Events.InfBleModuleCreated, $"Method Created() completed successfully.");
        }

        private void validateConfig(BleConfig m_Config)
        {
            if (m_Config.FilterConfig == null)
            {
                //
                //Validate the testing scenario of the Ble configuration
                if (m_Config.TestingOptions != null && m_Config.TestingOptions.AddMissingSections != null)
                {
                    if (m_Config.TestingOptions.AddMissingSections.IsEnabled)
                    {
                        if (m_Config.FilterConfig.Manufacturers != null && m_Config.FilterConfig.Manufacturers.Length > 0)
                            throw new Exception("Manufacturers filter can't be set if the configuration option TestingOptions.AddMissingSections is enabled. Please correct the configuration of the gateway.");

                        if (String.IsNullOrEmpty(m_Config.TestingOptions.AddMissingSections.LocalName) || m_Config.TestingOptions.AddMissingSections.LocalName.Length != 3)
                            throw new Exception("Configuration option TestingOptions.AddMissingSections.LocalName must be defined and must be exactly 3 characters long.");
                    }
                }
                else
                {
                    throw new Exception("Missing configuration setting \"FilterConfig\". This is required to start the Gateway with the BlouetoothReceive Module. Please correct the configuration of the gateway.");
                    
                }
            }
            else
            {
                //// Validate further BleFilterConfig child properties here if required.
                // ...
            }
        }

        /// <summary>
        /// Destroys the module by stopping the watchers
        /// </summary>
        public void Destroy()
        {
            this.m_Broker.Logger.Log(LogLevel.Debug, Events.InfBleModuleDestroyEntered, $"Destroy method entered.");

            //Takes each watcher from list of watchers and destroy
            foreach (var watcher in m_BleWatchers)
            {
                watcher.Value.Stop();
            }

            m_Task = null;

            this.m_Broker.Logger.Log(LogLevel.Debug, Events.InfBleModuleDestroyEntered, $"{nameof(BleModule)} destroyed.");
        }

        /// <summary>
        /// Starts BLE Watcher
        /// </summary>
        public void Start()
        {
            try
            {
                this.m_Broker.Logger.Log(LogLevel.Debug, Events.TraceBleModuleReceiverLoopStarted, $"{nameof(BleModule)} message receiver loop started.");
                //
                //Runs the Ble Module receiver on a new thread
                m_Task = new Task(new Action(this.bleReceiverLoop));

                m_Task.Start();

                m_Task.ContinueWith((r) =>
                {
                    var i = r;
                });
            }
            catch (AggregateException ae)
            {

            }
        }

        public void Receive(Message received_message)
        {
            //System.Diagnostics.Debug.WriteLine($"BleModule1: message \"{received_message.Properties["MessageId"].ToString()}\" retrieved!");
            //this.broker.Publish(received_message);
        }

        private void bleReceiverLoop()
        {
            try
            {
                //
                //Checks if the configuration exists
                if (m_Config != null)
                {
                    if (m_Config.FilterConfig.Manufacturers == null || m_Config.FilterConfig.Manufacturers.Count() == 0)
                    {
                        m_Config.FilterConfig.Manufacturers = new Daenet.IotGateway.Common.Entities.Config.Manufacturers[]
                        {
                           new Daenet.IotGateway.Common.Entities.Config.Manufacturers(){  Name = null,},
                        };
                    }

                    this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfManufacturersCount, $"{nameof(BleModule)} registered listener for {m_Config.FilterConfig.Manufacturers.Count()} manufacturers.");

                    // if (m_Config.Manufacturers != null && m_Config.Manufacturers.Count() > 0)

                    //
                    //For each manufacturer in configuration creates a new instance of Watcher
                    foreach (var manufacturer in m_Config.FilterConfig.Manufacturers)
                    {
                        this.m_Broker.Logger.Log(LogLevel.Trace, Events.ErrAdvFailed, $"{nameof(BleModule)} registered manufacturer '{manufacturer.Name}'.");

                        BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
                        if (!m_BleWatchers.ContainsKey(manufacturer.Name))
                        {
                            m_BleWatchers.Add(manufacturer.Name, watcher);
                            if (m_Config.FilterConfig != null && m_Config.FilterConfig.SignalStrength.HasValue)
                                watcher.SignalStrengthFilter.InRangeThresholdInDBm = m_Config.FilterConfig.SignalStrength;
                            if (manufacturer.Name != null)
                                watcher.AdvertisementFilter.Advertisement.LocalName = manufacturer.Name;
                            addPatternFilterIfRequired(watcher);
                            watcher.Received += Watcher_Received;
                            watcher.Stopped += Watcher_Stopped;
                            //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                            watcher.Start();
                        }
                    }
                    // }
                    //else
                    //{
                    //    m_BleWatcher = new BluetoothLEAdvertisementWatcher();
                    //    if (m_Config.InRangeSignal != null)
                    //        m_BleWatcher.SignalStrengthFilter.InRangeThresholdInDBm = Int16.Parse(m_Config.InRangeSignal);
                    //    addPatternFilterIfRequired(m_BleWatcher);
                    //    m_BleWatcher.Received += Watcher_Received;
                    //    m_BleWatcher.Stopped += Watcher_Stopped;
                    //    //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                    //    m_BleWatcher.Start();
                    //}

                }
                //else
                //{
                //    m_BleWatcher = new BluetoothLEAdvertisementWatcher();
                //    addPatternFilterIfRequired(m_BleWatcher);
                //    m_BleWatcher.Received += Watcher_Received;
                //    m_BleWatcher.Stopped += Watcher_Stopped;
                //    //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                //    m_BleWatcher.Start();
                //}
            }
            catch (Exception ex)
            {
                this.m_Broker.Logger.Log(LogLevel.Error, Events.ErrRegFailed, $"{nameof(BleModule)} registration of manufacturer faild.'.", ex);
            }
        }

        /*
        private void threadBodyBackgroundWatcher()
        {
            try
            {
                if (m_Config != null)
                {
                    if (m_Config.Manufacturers != null && m_Config.Manufacturers.Count() > 0)
                    {
                        foreach (var manufacturer in m_Config.Manufacturers)
                        {
                            BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
                            if (m_Config.InRangeSignal != null)
                                watcher.SignalStrengthFilter.InRangeThresholdInDBm = Int16.Parse(m_Config.InRangeSignal);
                            if (manufacturer.Name != null)
                                watcher.AdvertisementFilter.Advertisement.LocalName = manufacturer.Name;
                            addPatternFilterIfRequired(watcher);
                            watcher.Received += Watcher_Received;
                            watcher.Stopped += Watcher_Stopped;
                            //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                            watcher.Start();
                        }
                    }
                    else
                    {
                        BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
                        if (m_Config.InRangeSignal != null)
                            watcher.SignalStrengthFilter.InRangeThresholdInDBm = Int16.Parse(m_Config.InRangeSignal);
                        addPatternFilterIfRequired(watcher);
                        watcher.Received += Watcher_Received;
                        watcher.Stopped += Watcher_Stopped;
                        //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                        watcher.Start();
                    }
                }
                else
                {
                    BluetoothLEAdvertisementWatcher watcher = new BluetoothLEAdvertisementWatcher();
                    addPatternFilterIfRequired(watcher);
                    watcher.Received += Watcher_Received;
                    watcher.Stopped += Watcher_Stopped;
                    //watcher.ScanningMode = BluetoothLEScanningMode.Active;
                    watcher.Start();
                }
            }
            catch (Exception ex)
            {
                //Log/Trace
            }
        }
        */
        /// <summary>
        /// Event handler for stopping the watcher
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            this.m_Broker.Logger.Log(LogLevel.Debug, Events.InfWatcherStopped, $"{nameof(BleModule)} Watcher stopped.");
        }

        /// <summary>
        /// Adds the pattern filter for testing scenario
        /// </summary>
        /// <param name="watcher"></param>
        private void addPatternFilterIfRequired(BluetoothLEAdvertisementWatcher watcher)
        {
            if (m_Config.PatternFilters != null)
            {
                foreach (var patternFilter in m_Config.PatternFilters)
                    watcher.AdvertisementFilter.BytePatterns.Add(new BluetoothLEAdvertisementBytePattern(patternFilter.DataSectionType, patternFilter.Offset, Encoding.ASCII.GetBytes(patternFilter.PatternToLookFor).AsBuffer()));
            }
        }

        /// <summary>
        /// Event handler for BLE Message received event
        /// </summary>
        /// <param name="sender">BLE Watcher</param>
        /// <param name="args">Message</param>
        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfAdvReceived, $"{nameof(BleModule)} received advertisement '{args.Advertisement.LocalName}'.");

            try
            {
                byte[] messageData = getDataAsArray(args.Advertisement.DataSections.ToArray());

                this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfReceived, $"Package Received on : {args.Timestamp.DateTime} Signal Strength : {args.RawSignalStrengthInDBm.ToString()}");

                // If filtering pattern is set then we are running in test mode and trying to retrieve BLE advertising packets from emulators.
                // Due to the limitations of the current Bluetooth implementation in UWP, the sender in emulator was not able to set LocalName and Flags.
                // So, in this case and if the retrieved BLE packet contains one data section only (manufacturer specific), we should extend the packet with 
                // missing sections.
                bool shouldIgnoreMessage = false;
                if (m_Config.TestingOptions != null && m_Config.TestingOptions.AddMissingSections != null && m_Config.TestingOptions.AddMissingSections.IsEnabled)
                    messageData = addMissingBleDataSectionsIfRequired(args.Advertisement.DataSections.ToArray(), messageData, out shouldIgnoreMessage);

                if (!shouldIgnoreMessage)
                {
                    Dictionary<string, string> msgProps = new Dictionary<string, string>();
                    msgProps.Add("MessageId", Guid.NewGuid().ToString());
                    msgProps.Add("ReceivedAt", args.Timestamp.DateTime.ToString("yyyyMMdd_HHmmss"));
                    msgProps.Add("BluetoothAddress", args.BluetoothAddress.ToString());
                    msgProps.Add("Rssi", args.RawSignalStrengthInDBm.ToString());
                    msgProps.Add("IsFromBle", bool.TrueString);
                    Message messageToPublish = new Message(messageData, msgProps);
                    this.m_Broker.Publish(messageToPublish);
                }
            }
            catch (Exception ex)
            {
                this.m_Broker.Logger.Log(LogLevel.Warning, Events.ErrAdvFailed, $"{nameof(BleModule)} received advertisement '{args.Advertisement.LocalName}'.", ex);
            }
        }

        private byte[] getDataAsArray(BluetoothLEAdvertisementDataSection[] dataSections)
        {
            //
            //Converts the received bluetooth advertisement data sections from Ble Watcher into Array
            byte[] bleDataArray = new byte[0];

            foreach (var dataSection in dataSections)
            {
                var arrData = dataSection.Data.ToArray();
                bleDataArray = bleDataArray.Concat(arrData).ToArray();
            }

            return bleDataArray;
        }

        private bool checkSectionConditions(BluetoothLEAdvertisementDataSection[] dataSections)
        {
            foreach (var dataSection in dataSections)
            {
                if (m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.DataSectionType == dataSection.DataType)
                {
                    byte[] bytesToCompare = new byte[m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor.Length];
                    dataSection.Data.CopyTo((uint)m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.Offset, bytesToCompare, 0, m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor.Length);
                    if (bytesToCompare.SequenceEqual(Encoding.ASCII.GetBytes(m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor)))
                        return true;
                }
            }

            return false;
        }


        /// <summary>
        /// For test purposes (to simulate multiple BLE tools) we have implemented daenet Daenet.BleEmulator which is able to simulate sending of 
        /// BLE packets similar to the packets being sent by real EWS BLE modules.
        /// According to the EWS BLE spec, in addition to the manufacturer specific data section with telemetry data, the EWS BLE base packet also contains
        /// the BLE Advertising data sections "Flags" and "LocalName".
        /// Due some restrictions in the current version of the UWP BLE Publisher Library it is not possible to create these sections using .NET code.
        /// In order to be able to retrieve packets without those sections we can use this method to extends data with the both BLE Advertising data sections 
        /// "Flags" and "LocalName". 
        /// </summary>
        /// <returns>Byte array containing the BLE advertising sections "LocalName" and "Flags"</returns>
        private byte[] addMissingBleDataSectionsIfRequired(BluetoothLEAdvertisementDataSection[] dataSections, byte[] bleDataArray, out bool ignoreMessageWhileUnexpectedType)
        {
            this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfMissingSection, $"TestingOption AddMissingSection: checking if we have to add missing sections to the message ...'.");

            if (m_Config.TestingOptions.AddMissingSections.IgnoreUnknownPackages)
                ignoreMessageWhileUnexpectedType = true;
            else
                ignoreMessageWhileUnexpectedType = false;

            List<byte> baseBlepacketMissingSections = new List<byte>();
            if (m_Config.TestingOptions != null && m_Config.TestingOptions.AddMissingSections != null && m_Config.TestingOptions.AddMissingSections.IsEnabled)
            {
                if (m_Config.TestingOptions.AddMissingSections.Conditions.ExpectedTotalDataLength == bleDataArray.Length)
                {
                    if (m_Config.TestingOptions.AddMissingSections.Conditions.ExpectedNumberOfDataSections == dataSections.Length)
                    {
                        if (checkSectionConditions(dataSections))
                        {
                            // Add the flags (length=2, type=1, value=6)
                            baseBlepacketMissingSections.AddRange(new byte[] { /*2, 1,*/ m_Config.TestingOptions.AddMissingSections.Flags });

                            // Add LocalName/Manufacturer (length=4, type=9, value=EWS)
                            //baseBlepacketMissingSections.AddRange(new byte[] { 4, 9 });
                            baseBlepacketMissingSections.AddRange(Encoding.ASCII.GetBytes(m_Config.TestingOptions.AddMissingSections.LocalName));

                            // Remove first two bytes, somehow they were appended to the message
                            var list = bleDataArray.ToList();
                            list.RemoveAt(0);
                            list.RemoveAt(0);
                            bleDataArray = baseBlepacketMissingSections.ToArray().Concat(list.ToArray()).ToArray();
                            ignoreMessageWhileUnexpectedType = false;

                            this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfCheckSectionConditions, $"TestingOption AddMissingSection: Message extended with the sections \"LocalName\"=\"{m_Config.TestingOptions.AddMissingSections.LocalName}\" and \"Flags\"=\"{m_Config.TestingOptions.AddMissingSections.Flags.ToString()}\".");
                        }
                        else
                            this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfCheckSectionConditionsNotSatisfied, $"TestingOption AddMissingSection: The message will not be extended due mismatch the Pattern condition. Could not find expected value \"{m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor}\" at the offset {m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.Offset.ToString()} of the data section with the type {m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.DataSectionType.ToString()}.");
                    }
                    else
                        this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfExpectedNumberOfDataSections, $"TestingOption AddMissingSection: The message will not be extended due mismatch in ExpectedNumberOfDataSections. Expected value is {m_Config.TestingOptions.AddMissingSections.Conditions.ExpectedNumberOfDataSections.ToString()} retrieved value is {dataSections.Length.ToString()}.");
                }
                else
                    this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfExpectedNumberOfDataSections, $"TestingOption AddMissingSection: The message will not be extended due mismatch in ExpectedTotalDataLength. Expected value is {m_Config.TestingOptions.AddMissingSections.Conditions.ExpectedTotalDataLength.ToString()} retrieved value is {bleDataArray.Length.ToString()}.");

            }

            if (ignoreMessageWhileUnexpectedType)
                this.m_Broker.Logger.Log(LogLevel.Trace, Events.InfIgnoreMessageWhileUnexpectedType, $"TestingOption AddMissingSection: The message will not be processed due unexpected/unknown message type.");

            return bleDataArray;
        }
    }
}
