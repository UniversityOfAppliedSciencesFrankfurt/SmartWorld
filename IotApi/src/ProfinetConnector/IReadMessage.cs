using Dacs7.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProfinetConnector
{
    public interface IReadMessage
    {
        [Required]
        PlcArea Area { get; set; }

        [Required]
        int Offset { get; set; }

        [Required]
        Type Type { get; set; }

        [Required]
        int[] Arg { get; set; }
    }
}
