using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseListDTO
    {
        public int Total { get; set; }
        public IEnumerable<HorseDTO> Horses { get; set; }
    }
}
