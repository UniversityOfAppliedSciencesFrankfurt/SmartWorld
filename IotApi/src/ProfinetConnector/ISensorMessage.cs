using Dacs7.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProfinetConnector
{
    public interface ISensorMessage
    {
        PlcArea Area { get; set; }
        int Offset { get; set; }
        object Value { get; set; }
        int[] Args { get; set; }
    }
}
