using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Ancestry : IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string AncestorOId { get; set; }
        public double AvgMC { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [NotMapped]
        public string Name { get; set; }
        [NotMapped]
        public string Sex { get; set; }
        [NotMapped]
        public int Age { get; set; }
        [NotMapped]
        public string Country { get; set; }
        [NotMapped]
        public string MtDNATitle { get; set; }
        [NotMapped]
        public string MtDNAColor { get; set; }
    }
}
