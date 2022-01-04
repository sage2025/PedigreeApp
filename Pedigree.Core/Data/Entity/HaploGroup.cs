using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class HaploGroup : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        public string Color { get; set; }
        public List<HaploType> Types { get; set; }

        [NotMapped]
        public int RefPopCount { get; set; }
        [NotMapped]
        public float RefPopCountPercent { get; set; }
        [NotMapped]
        public int RatedHorses { get; set; }
        [NotMapped]
        public float RatedHorsesPercent { get; set; }
        [NotMapped]
        public int ThreePlusStarts { get; set; }
        [NotMapped]
        public float ThreePlusStartsPercent { get; set; }
        [NotMapped]
        public int Elite { get; set; }
        [NotMapped]
        public float ElitePercent { get; set; }
        [NotMapped]
        public int NonElite { get; set; }
        [NotMapped]
        public float NonElitePercent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
