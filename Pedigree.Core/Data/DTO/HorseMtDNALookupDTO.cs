using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseMtDNALookupDTO
    {
        public int Id { get; set; }
        public string OId { get; set; }
        public string Name { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string Country { get; set; }
        public string Family { get; set; }
        public string MtDNATitle { get; set; }
        public string MtDNAColor { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public int G1Wnrs { get; set; }
        public int G2Wnrs { get; set; }
        public int G3Wnrs { get; set; }
        public int LRWnrs { get; set; }
        public int TotalWnrs { get; set; }
    }
}
