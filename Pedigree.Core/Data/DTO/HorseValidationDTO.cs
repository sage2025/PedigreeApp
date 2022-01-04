using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseValidationDTO : BasicResponseDTO
    {
        public bool IsYoungerParent { get; set; }
        public bool IsOverThresholdOldParent { get; set; }

    }
}
