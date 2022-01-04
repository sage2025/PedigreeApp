using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class MLModel : IEntity
    {
        [Key]
        public int Id { get; set; }

        public int HorsesCount { get; set; }
        public string ModelName { get; set; }
        public double RMSError { get; set; }
        public double RSquared { get; set; }
        public int ModelVersion { get; set; }
        public string ModelPath { get; set; }
        public string Features { get; set; }
        public bool Deployed { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
