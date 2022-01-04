using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class HorseHeirarchy : Horse
    {
        public int OffspringId { get; set; }
        public int Depth { get; set; }
        public string SD { get; set; }
        public int Seq { get; set; }
    }

    public class HorseHeirarchySingleRecord : HorseHeirarchy
    {
        public IEnumerable<HorseHeirarchySingleRecord> Children { get; set; }
    }

}
