using Dacs7.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProfinetConnector
{
    public interface ISensorMessage
    {
        [Required]
        PlcArea Area { get; set; }

        [Required]
        int Offset { get; set; }

        [Required]
        object Value { get; set; }

        [Required]
        int[] Args { get; set; }
    }
}
