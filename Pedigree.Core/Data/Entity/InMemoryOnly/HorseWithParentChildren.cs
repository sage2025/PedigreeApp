using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class HorseWithParentChildren
    {
        public Horse MainHorse { get; set; }
        public IEnumerable<Horse> Parents { get; set; }

        public IEnumerable<Horse> Children { get; set; }
    }
}
