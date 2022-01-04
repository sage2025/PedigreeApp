using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
   public class StallionRatingsWithTotal
    {
       public IEnumerable<StallionRating> StallionRatings { get; set; }
        public int Total { get; set; }

    }
}
