using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class AuctionDetail : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AuctionId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        public int LotNumber { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public int YOB { get; set; }
        public string Sex { get; set; }

        public string Country { get; set; }

        public int SireId { get; set; }
        public int DamId { get; set; }

        public int mtDNAHapId { get; set; }

        public double mlScore { get; set; }
        [NotMapped]
        public int HorseId { get; set; }
        [NotMapped]
        public string FatherName { get; set; }
        [NotMapped]
        public string MotherName { get; set; }
        [NotMapped]
        public MtDNAHap mtDNAHap { get; set; }

    }
}
