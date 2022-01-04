using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedigree.App.Requests
{
    public class RelationshipParentTypeRequest
    {
        public string HorseOId { get; set; }
        public string ParentType { get; set; }
    }
}
