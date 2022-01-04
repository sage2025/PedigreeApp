using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class SireCrossData
    {
        public Horse[] Sires1 { get; set; }
        public Horse[] Sires2 { get; set; }
        public List<Horse>[] Crosses { get; set; }
    }
}
