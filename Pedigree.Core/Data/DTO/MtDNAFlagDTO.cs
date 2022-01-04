using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class MtDNAFlagDTO
    {
        public int Id { get; set; }
        public string StartHorseOId{ get; set; }
        public string EndHorseOId { get; set; }

        
        public int StartHorseId { get; set; }        
        public string StartHorseName { get; set; }        
        public int StartHorseAge { get; set; }
        public string StartHorseCountry { get; set; }

        public int EndHorseId { get; set; }
        public string EndHorseName { get; set; }
        public int EndHorseAge { get; set; }
        public string EndHorseCountry { get; set; }
    }
}
