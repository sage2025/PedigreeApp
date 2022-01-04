using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class StallionRatingListDTO
    {
        public int Total { get; set; }
        public IEnumerable<StallionRatingDTO> Data { get; set; }
    }
}
