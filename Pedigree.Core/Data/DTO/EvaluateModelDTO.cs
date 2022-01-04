using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class EvaluateModelDTO
    {
        [Required]
        public int ModelId { get; set; }
        [Required]
        public Dictionary<string, string> Data { get; set; }
    }
}
