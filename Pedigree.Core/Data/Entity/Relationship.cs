using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Relationship : IEntity
    {
        [Key]
        public int Id { get; set; }
        
        public string HorseOId { get; set; }

        public string ParentOId { get; set; }
        public string ParentType { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
