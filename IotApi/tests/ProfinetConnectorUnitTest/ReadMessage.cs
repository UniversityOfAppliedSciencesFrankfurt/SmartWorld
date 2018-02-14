using System;
using System.Collections.Generic;
using System.Text;
using Dacs7.Domain;
using ProfinetConnector;

namespace ProfinetConnectorUnitTest
{
    class ReadMessage : IReadMessage
    {
        public PlcArea Area { get; set; }
        public int Offset { get; set; }
        public Type Type { get; set; }
        public int[] Args { get; set; }
    }
}
