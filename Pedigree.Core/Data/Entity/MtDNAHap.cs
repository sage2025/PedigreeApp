using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class MtDNAHap
    {

        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Color { get; set; }
        
    }
}
