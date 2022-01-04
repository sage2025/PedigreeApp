using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Position : IEntity
    {
        [Key]
        public int Id { get; set; }
        public int RaceId { get; set; }
        public Race Race { get; set; }
        public string HorseOId { get; set; }
        public int Place { get; set; }

        [NotMapped]
        public int HorseId { get; set; }
        [NotMapped]
        public string HorseName { get; set; }
        [NotMapped]
        public int HorseAge { get; set; }
        [NotMapped]
        public string HorseSex { get; set; }
        [NotMapped]
        public string HorseCountry { get; set; }
        [NotMapped]
        public string HorseFamily { get; set; }
        [NotMapped]
        public string HorseFather { get; set; }
        [NotMapped]
        public string HorseMother { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
