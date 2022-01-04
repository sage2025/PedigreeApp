using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HaploGroupDistanceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Color { get; set; }
        public float FirstPlaceCount { get; set; }
        public float SprintPercent { get; set; }
        public float SprinterMilerPercent { get; set; }
        public float IntermediatePercent { get; set; }
        public float LongPercent { get; set; }
        public float ExtendedPercent { get; set; }
    }
}
