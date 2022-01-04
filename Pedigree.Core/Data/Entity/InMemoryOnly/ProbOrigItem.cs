using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class ProbOrigItem
    {
        public double X { get; set; }       // Total
        public double Y { get; set; }       // Marginal
        public double Cum { get; set; }     // Cumulated
        public double Xmin { get; set; }    // LowerBound
        public double Xmaj { get; set; }    // UpperBound
    }
}
