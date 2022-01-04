using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseHeirarchyDataDTO
    {
        public int Id { get; set; }

        public string OId { get; set; }
        public string Name { get; set; }

        public int Age { get; set; }
        public string Sex { get; set; }
        public string Country { get; set; }
        public string Family { get; set; }
        public int? MtDNA { get; set; }
        public string MtDNATitle { get; set; }
        public string MtDNAColor { get; set; }
        public string BestRaceClass { get; set; }
        public int FatherId { get; set; }
        public string FatherOId { get; set; }
        public string FatherName { get; set; }
        public int MotherId { get; set; }
        public string MotherName { get; set; }
        public string MotherOId { get; set; }

        public int Depth { get; set; }
        public string SD { get; set; }
        public string BgColor { get; set; }
        public double Coi { get; set; }
        public double Pedigcomp { get; set; }
        public double Gi { get; set; }
        public double? ZHistoricalBPR { get; set; } = null;
        public double? ZCurrentBPR { get; set; } = null;
        public double? Bal { get; set; } = null;
        public double? AHC { get; set; } = null;
        public double? Kal { get; set; } = null;

        public List<HorseHeirarchyDataDTO> Children { get; set; } = new List<HorseHeirarchyDataDTO>();
        public bool IsLeaf
        {
            get
            {
                return (Children.Count < 1);
            }
        }
    }
}
