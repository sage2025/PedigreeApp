using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Weight : IEntity
    {
        [Key]
        public int Id { get; set; }

        public string Country { get; set; }
        public string Distance { get; set; }
        public string Surface { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public Double Value { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
