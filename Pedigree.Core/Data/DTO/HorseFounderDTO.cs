using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseFounderDTO
    {
        [Required]
        public bool IsFounder { get; set; }
    }
}
