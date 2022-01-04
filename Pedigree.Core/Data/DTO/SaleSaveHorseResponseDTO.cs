using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class SaleSaveHorseResponseDTO
    {
        public int Number { get; set; }
        public int HorseId { get; set; }
        public int SireId { get; set; }
        public int DamId { get; set; }
        public double Pedigcomp { get; set; }
        public int? MtDNA { get; set; }
        public string MtDNATitle { get; set; }
        public string MtDNAColor { get; set; }
        public double MLScore { get; set; }
    }
}
