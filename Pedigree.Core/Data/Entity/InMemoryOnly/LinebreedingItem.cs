using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class LinebreedingItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> Stats { get; set; }
        public int Crosses { get; set; }
        public double Inbreed { get; set; }
        public double Relation { get; set; }
    }
}
