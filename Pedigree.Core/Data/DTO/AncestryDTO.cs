using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class AncestryDTO
    {
        public int Id { get; set; }
        public string AncestorOId{ get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public string MtDNATitle { get; set; }
        public string MtDNAColor { get; set; }
        public double AvgMC { get; set; }
    }
}
