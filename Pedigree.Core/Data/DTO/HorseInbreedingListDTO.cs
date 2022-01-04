using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseInbreedingListDTO
    {
        public int Total { get; set; }
        public IEnumerable<HorseInbreedingDTO> Horses { get; set; }
    }
}
