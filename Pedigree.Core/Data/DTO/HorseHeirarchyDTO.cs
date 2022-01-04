using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseHeirarchyDTO
    {
        public List<HorseHeirarchyDataDTO> Heirarchy { get; set; } = new List<HorseHeirarchyDataDTO>();
    }
}
