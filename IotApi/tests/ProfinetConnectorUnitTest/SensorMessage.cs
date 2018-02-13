using System;
using System.Collections.Generic;
using System.Text;
using Dacs7.Domain;
using ProfinetConnector;

namespace ProfinetConnectorUnitTest
{
    public class SensorMessage : ISensorMessage
    {
        public PlcArea Area { get; set; }
        public int Offset { get; set; }
        public object Value { get; set; }
        public int[] Args { get; set; }
    }
}
