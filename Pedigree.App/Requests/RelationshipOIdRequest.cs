using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pedigree.App.Requests
{
    public class RelationshipOIdRequest
    {
        public string HorseOId { get; set; }
        public string ParentOId { get; set; }
    }
}
