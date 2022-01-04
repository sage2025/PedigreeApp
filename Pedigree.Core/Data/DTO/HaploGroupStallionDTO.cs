using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HaploGroupStallionDTO
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

        public int G1Wnr { get; set; }
        public float G1WnrPercent { get; set; }
        public int G2Wnr { get; set; }
        public float G2WnrPercent { get; set; }
        public int G3Wnr { get; set; }
        public float G3WnrPercent { get; set; }
        public int SWnr { get; set; }
        public float SWnrPercent { get; set; }
    }
}
