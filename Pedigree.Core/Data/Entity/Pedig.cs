using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Innofactor.EfCoreJsonValueConverter;
using Pedigree.Core.Data.Entity.InMemoryOnly;

namespace Pedigree.Core.Data.Entity
{
    public class Pedig: IEntity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string HorseOId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonField] //<------ Add this attribute
        public List<ProbOrig> ProbOrigs { get; set; }
        public DateTime? ProbOrigsUpdatedAt { get; set; } = null;

        [JsonField]
        public List<HorseHeirarchy> Pedigree { get; set; }
        public DateTime? PedigreeUpdatedAt { get; set; } = null;

    }

    public class ProbOrig
    {
        public string AncestorOId { get; set; }
        public double MC { get; set; }
    }
}
