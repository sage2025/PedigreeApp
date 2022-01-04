using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class PlotItem
    {
        public string Category { get; set; }
        public string Title { get; set; }
        public double Q1 { get; set; }
        public double Median { get; set; }
        public double Q3 { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int Info { get; set; }
    }
}
