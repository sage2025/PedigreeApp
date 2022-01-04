using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class HorseTwinDTO
    {
        public int Id { get; set; }

        public string OId { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Sex { get; set; }
        public string Country { get; set; }
        public string Family { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public int TwinYear { get; set; }
        public int Twins { get; set; }
    }
}
