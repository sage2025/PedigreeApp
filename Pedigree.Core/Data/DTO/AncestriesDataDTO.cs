using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class AncestriesDataDTO
    {
        public int HorsesCount { get; set; }
        public int AncestorsCount { get; set; }
        public double GenomePercent { get; set; }

        public List<AncestryDTO> Ancestries { get; set; }

    }
}
