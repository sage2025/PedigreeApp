using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class HorseTwin : Horse
    {
        public int TwinYear { get; set; }
        public int Twins { get; set; }
    }

}
