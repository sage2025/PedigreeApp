using Pedigree.Core.Data.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class AuctionDetailDTO
    {
        public int Id { get; set; }
        public int AuctionId { get; set; }
        public int LotNumber { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public int YOB { get; set; }
        public string Sex { get; set; }
        public string Country { get; set; }
        public int SireId { get; set; }
        public int DamId { get; set; }
        public int mtDNAHapId { get; set; }
        public double Pedigcomp { get; set; }
        public int HorseId { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public double mlScore { get; set; }
        public MtDNAHap mtDNAHap { get; set; }

    }
}
