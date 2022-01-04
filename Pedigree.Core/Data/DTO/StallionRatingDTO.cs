using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class StallionRatingDTO
    {
        public string HorseOId { get; set; }
        public string HorseName { get; set; }
        public int? CropAge { get; set; } = null;
        public int? RCount { get; set; } = null; 
        public int? SCount { get; set; } = null;
        public int? ZCount { get; set; } = null;
        public double? Rating { get; set; } = null;
        public double? IV { get; set; } = null;
        public double? AE { get; set; } = null;
        public double? PRB2 { get; set; } = null;

    }
}
