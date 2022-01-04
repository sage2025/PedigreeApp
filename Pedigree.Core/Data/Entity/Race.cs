using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Race : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        
        [Column(TypeName = "date")] 
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public string Distance { get; set; }
        public string Surface { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public Double Weight { get; set; } 
        [NotMapped]
        public Double BPR { get; set; }
        [NotMapped]
        public int Rnrs { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
