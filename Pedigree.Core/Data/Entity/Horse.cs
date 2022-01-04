using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Horse :IEntity
    {
        [Key]
        public int Id { get; set; }

        public string OId { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
        public string Sex { get; set; }
        public string Country { get; set; }
        public string Family { get; set; }
        public bool IsFounder { get; set; }
        public bool isDeleted { get; set; }
        public int? MtDNA { get; set; }
        public bool MtDNAFlag { get; set; }
        public HaploType HaploType { get; set; }

        [NotMapped]
        public string MtDNATitle { get; set; }
        [NotMapped]
        public string MtDNAColor { get; set; }
        [NotMapped]
        public int HaploGroupId { get; set; }
        [NotMapped]
        public string BestRaceClass { get; set; }

        [NotMapped]
        public int FatherId { get; set; }
        [NotMapped]
        public string FatherName { get; set; }
        [NotMapped]
        public string FatherOId { get; set; }
        [NotMapped]
        public Horse Father { get; set; }

        [NotMapped]
        public int MotherId { get; set; }
        [NotMapped]
        public string MotherName { get; set; }
        [NotMapped]
        public string MotherOId { get; set; }
        [NotMapped]
        public Horse Mother { get; set; }

        [NotMapped]
        public int BmFatherId { get; set; }
        [NotMapped]
        public string BmFatherName { get; set; }
        [NotMapped]
        public string BmFatherOId { get; set; }

        [NotMapped]
        public double Coi { get; set; }
        [NotMapped]
        public double Pedigcomp { get; set; }
        [NotMapped]
        public double Gi { get; set; }
        [NotMapped]
        public double? HistoricalBPR { get; set; } = null;
        [NotMapped]
        public double? ZHistoricalBPR { get; set; } = null;
        [NotMapped]
        public double? CurrentBPR { get; set; } = null;
        [NotMapped]
        public double? ZCurrentBPR { get; set; } = null;
        [NotMapped]
        public double? Bal { get; set; } = null;
        [NotMapped]
        public double? AHC { get; set; } = null;
        [NotMapped]
        public double? Kal { get; set; } = null;

        [NotMapped]
        public int G1Wnrs { get; set; }
        [NotMapped]
        public int G2Wnrs { get; set; }
        [NotMapped]
        public int G3Wnrs { get; set; }
        [NotMapped]
        public int LRWnrs { get; set; }
        [NotMapped]
        public int TotalWnrs { get; set; }

        [NotMapped]
        public bool HasRelationships { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Guid UpdatedBy { get; set; } = Guid.Empty;
        public Guid CreatedBy { get; set; } = Guid.Empty;


    }
}
