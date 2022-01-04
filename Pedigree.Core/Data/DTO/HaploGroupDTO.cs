using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HaploGroupDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public HaploTypeDTO[] Types { get; set; }

        public int RefPopCount { get; set; }
        public float RefPopCountPercent { get; set; }
        public int RatedHorses { get; set; }
        public float RatedHorsesPercent { get; set; }
        public int ThreePlusStarts { get; set; }
        public float ThreePlusStartsPercent { get; set; }
        public int Elite { get; set; }
        public float ElitePercent { get; set; }
        public int NonElite { get; set; }
        public float NonElitePercent { get; set; }
    }
}
