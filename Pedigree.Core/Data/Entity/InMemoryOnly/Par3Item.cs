using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class Par3Item
    {
        public int HorseId1 { get; set; }
        public string HorseName1 { get; set; }
        public int HorseId2 { get; set; }
        public string HorseName2 { get; set; }
        public double Coi { get; set; }
        public List<string> CommonAncestors { get; set; }


    }
}
