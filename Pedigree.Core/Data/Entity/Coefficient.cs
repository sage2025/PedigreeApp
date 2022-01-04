using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Coefficient : IEntity
    {
        public int Id { get; set; }
        public string HorseOId { get; set; }
        public double COI { get; set; }
        public double COI1 { get; set; }
        public double COI2 { get; set; }
        public double COI3 { get; set; }
        public double COI4 { get; set; }
        public double COI5 { get; set; }
        public double COI6 { get; set; }
        public double COI7 { get; set; }
        public double COI8 { get; set; }
        public double Pedigcomp { get; set; }
        public double GI { get; set; }
        public double GDGS { get; set; }
        public double GDGD { get; set; }
        public double GSSD { get; set; }
        public double GSDD { get; set; }
        public DateTime? COIUpdatedAt { get; set; } = null;

        // SPORT Values
        public double? HistoricalBPR { get; set; } = null;
        public double? HistoricalRD { get; set; } = null;
        public double? ZHistoricalBPR { get; set; } = null;
        public double? CurrentBPR { get; set; } = null;
        public double? CurrentRD { get; set; } = null;
        public double? ZCurrentBPR { get; set; } = null;
        public DateTime? BPRUpdatedAt { get; set; } = null;

        // COID
        public double COID1 { get; set; }
        public double COID2 { get; set; }
        public double COID3 { get; set; }
        public double COID4 { get; set; }
        public double COID5 { get; set; }
        public double COID6 { get; set; }

        // GRain
        public DateTime? GRainProcessStartedAt { get; set; } = null;
        public double? Bal { get; set; }
        public double? AHC { get; set; }
        public double? Kal { get; set; }
        public DateTime? GRainUpdatedAt { get; set; } = null;

        // UniqueAncestorsCount
        public int? UniqueAncestorsCount { get; set; }
        public DateTime? UniqueAncestorsCountUpdatedAt { get; set; } = null;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
