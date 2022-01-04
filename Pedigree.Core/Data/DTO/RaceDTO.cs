using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class RaceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public string Distance { get; set; }
        public string Surface { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Double Weight { get; set; }
        public int Rnrs { get; set; }
        public Double BPR { get; set; }
    }
}
