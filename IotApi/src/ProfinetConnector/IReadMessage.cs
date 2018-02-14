using Dacs7.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProfinetConnector
{
    public interface IReadMessage
    {
        PlcArea Area { get; set; }
        int Offset { get; set; }
        Type Type { get; set; }
        int[] Args { get; set; }
    }
}
