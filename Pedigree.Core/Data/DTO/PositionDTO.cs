using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class PositionDTO
    {
        public int Id { get; set; }
        public int RaceId { get; set; }
        public int Place { get; set; }
        public string HorseOId { get; set; }
        public int HorseId { get; set; }
        public string HorseName { get; set; }
        public int HorseAge { get; set; }
        public string HorseSex { get; set; }
        public string HorseCountry { get; set; }
        public string HorseFamily { get; set; }
        public string HorseFather { get; set; }
        public string HorseMother { get; set; }
    }
}
