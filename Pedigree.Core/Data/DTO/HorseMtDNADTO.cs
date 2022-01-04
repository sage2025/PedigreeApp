using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseMtDNADTO
    {
        [Required]
        public int MtDNA{ get; set; }
    }
}
