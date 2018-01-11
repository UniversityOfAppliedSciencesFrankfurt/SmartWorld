using Daenet.IotGateway.Common.Entities.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace Daenet.IotGateway.BluetoothLEModule
{
    public class BleConfig
    {
        public BleFilterConfig FilterConfig { get; set; }
            
        public PatternFilterConfig[] PatternFilters { get; set; }

        public TestingOptionsConfig TestingOptions { get; set; }
    }

    public class PatternFilterConfig
    {
        // BLE advertisement data section type to look for the byte pattern in.
        // Allowed are values 0-255. 
        public byte DataSectionType { get; set; }
        public string PatternToLookFor { get; set; }
        // Offset of pattern from the begin of the advertising data section.
        public short Offset { get; set; }
    }
    
    public class TestingOptionsConfig
    {

        public MissingDataSectionsConfig AddMissingSections { get; set; }
    }

    public class MissingDataSectionsConfig
    {
        public bool IsEnabled { get; set; }
        /// <summary>
        /// The string value defined by this property will be used ba module as filter. This means, only packets containing here 
        /// defined value in a part of one of the manufacturer data sections of the BLE packet will processed. All other BLE packets will be ignored.
        /// The main purpose of this setting option is testing, since the current version of BLE libraries in UWP is not able to set 
        /// LocalName (in case of EWS the manufacturer information is stored there) of the advertising packet.
        /// Leaving this setting option empty or null will cause the filtering logic to not be applied.
        /// </summary>
        public string LocalName { get; set; }
        public byte Flags { get; set; }
        public MissingDataConditions Conditions { get; set; }
        public bool IgnoreUnknownPackages { get; set; }
    }

    public class MissingDataConditions
    {
        public short ExpectedTotalDataLength { get; set; }
        public short ExpectedNumberOfDataSections { get; set; }

        public PatternFilterConfig Pattern { get; set; }
    }
}
