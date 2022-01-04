using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class HorseRace
    {
        public string HorseOId { get; set; }
        public int Place { get; set; }
        public int RaceId { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public string Distance { get; set; }
        public string Surface { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }        
        public Double BPR { get; set; }
        public int? MtDNA { get; set; }
    }
}
