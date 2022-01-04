using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class RelationshipDTO
    {
        public int Id { get; set; }
        public string HorseOId { get; set; }

        public string ParentOId { get; set; }
        public string ParentType { get; set; }
    }
}
