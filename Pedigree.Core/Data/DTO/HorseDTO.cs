using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseDTO
    {
        public int Id { get; set; }

        public string OId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Country { get; set; }
        public string Family { get; set; }
        public bool IsFounder { get; set; }
        public int? MtDNA { get; set; }
        public bool MtDNAFlag { get; set; }
        public string MtDNATitle { get; set; }
        public string MtDNAColor { get; set; }

        public string FatherOId { get; set; }
        public string FatherName { get; set; }
        public string MotherOId { get; set; }
        public string MotherName { get; set; }

        public string BmFatherOId { get; set; }     // Broodmare Sire
        public string BmFatherName { get; set; }

        public string BestRaceClass { get; set; }
        public List<HorseRaceDTO> Races { get; set; }

        public bool HasRelationships { get; set; }
        public bool ShowHeader { get; set; }
        public string ShowHeaderText { get; set; }
    }
}
