using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HaploTypeDTO
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string Name { get; set; }
        public int Population { get; set; }
        public float PopulationPercent { get; set; }
    }
}
