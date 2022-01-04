using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity
{
    public class Inbreed : IEntity
    {
        public int Id { get; set; }
        public string InbreedOId { get; set; }
        public string OId { get; set; }
        public string SD { get; set; }
        public string Depth { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
