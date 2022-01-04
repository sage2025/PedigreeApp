using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseWithParentChildrenDTO
    {

        public HorseDTO MainHorse { get; set; }
        public IEnumerable<HorseDTO> Parents { get; set; }

        public IEnumerable<HorseDTO> Children { get; set; }
    }
}
