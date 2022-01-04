using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class HaploType : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int GroupId { get; set; }

        [Required]
        public string Name { get; set; }

        [NotMapped]
        public int Population { get; set; }
        [NotMapped]
        public float PopulationPercent { get; set; }
        public HaploGroup Group { get; set; }
        public List<Horse> Horses { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
