using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
   public class HorsesWithTotal
    {
       public IEnumerable<Horse> Horses { get; set; }
        public int Total { get; set; }

    }
}
