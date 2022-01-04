using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class TrainModelDTO
    {
        [Required]
        public Dictionary<string, bool> Columns { get; set; }
    }
}
