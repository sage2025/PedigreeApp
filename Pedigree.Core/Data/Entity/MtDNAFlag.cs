using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class MtDNAFlag : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StartHorseOId { get; set; }
        public string EndHorseOId { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public int StartHorseId { get; set; }
        [NotMapped]
        public string StartHorseName { get; set; }
        [NotMapped]
        public int StartHorseAge { get; set; }
        [NotMapped]
        public string StartHorseCountry { get; set; }

        [NotMapped]
        public int EndHorseId { get; set; }
        [NotMapped]
        public string EndHorseName { get; set; }
        [NotMapped]
        public int EndHorseAge { get; set; }
        [NotMapped]
        public string EndHorseCountry { get; set; }

    }
}
