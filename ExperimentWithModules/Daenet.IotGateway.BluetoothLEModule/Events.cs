using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daenet.IotGateway.BluetoothLEModule
{
    /// <summary>
    /// Events logged by BLE module.
    /// </summary>
    internal static class Events
    {
        /// <summary>
        /// Method Create entered.
        /// </summary>
        public static int InfBleModuleCreateEntered = 101;

        /// <summary>
        /// Module has been created and configuration validated successfully.
        /// </summary>
        public static int InfBleModuleCreated = 102;

        /// <summary>
        /// Method Destroy entered.
        /// </summary>
        public static int InfBleModuleDestroyEntered = 103;

        /// <summary>
        /// Module Destroy completed successfully.
        /// </summary>
        public static int InfBleModuleDestroyed = 104;

        /// <summary>
        /// BLE message receiver loop started.
        /// </summary>
        public static int TraceBleModuleReceiverLoopStarted = 105;

        /// <summary>
        /// BLE event configured with number of manufacturers.
        /// </summary>
        public static int InfManufacturersCount = 106;

        /// <summary>
        /// Registered manufacturer.
        /// </summary>
        public static int InfRegManufacturer= 107;

        /// <summary>
        /// BLE advertisement event received.
        /// </summary>
        public static int InfAdvReceived = 108;

        /// <summary>
        ///   $"Package Received on : {args.Timestamp.DateTime} Signal Strength : {args.RawSignalStrengthInDBm.ToString()}"
        /// </summary>
        public static int InfReceived = 109;

    
        /// <summary>
        /// BLE advertisement event received.
        /// </summary>
        public static int ErrAdvFailed = 110;

        /// <summary>
        /// Registration failed.
        /// </summary>
        public static int ErrRegFailed = 111;

        /// <summary>
        /// Watcher stopped.
        /// </summary>
        public static int InfWatcherStopped = 112;

        /// <summary>
        /// TestingOption AddMissingSection: checking if we have to add missing sections to the message ...
        /// </summary>
        public static int InfMissingSection = 113;

        /// <summary>
        /// TestingOption AddMissingSection: Message extended with the sections
        /// </summary>
        public static int InfExtended = 114;

        /// <summary>
        /// TestingOption AddMissingSection: Message extended with the sections
        /// </summary>
        public static int InfCheckSectionConditions = 115;

        /// <summary>
        /// The message will not be extended due mismatch the Pattern condition. Could not find expected value \"{m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor}\" at the offset {m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.Offset.ToString()} of the data section with the type 
        /// </summary>
        public static int InfCheckSectionConditionsNotSatisfied = 116;


        /// <summary>
        /// The message will not be extended due mismatch the Pattern condition. Could not find expected value \"{m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.PatternToLookFor}\" at the offset {m_Config.TestingOptions.AddMissingSections.Conditions.Pattern.Offset.ToString()} of the data section with the type 
        /// </summary>
        public static int InfExpectedNumberOfDataSections = 117;

        /// <summary>
        /// TestingOption AddMissingSection: The message will not be extended due mismatch in ExpectedTotalDataLength. Expected value is {m_Config.TestingOptions.AddMissingSections.Conditions.ExpectedTotalDataLength.ToString()} retrieved value is {bleDataArray.Length.ToString()}.
        /// </summary>
        public static int InfUnexpectedTotalDataLength = 118;

        /// <summary>
        /// TestingOption AddMissingSection: The message will not be processed due unexpected/unknown message type.
        /// </summary>
        public static int InfIgnoreMessageWhileUnexpectedType = 119;
    }
}
