using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class BasicResponseDTO
    {
        public bool Output { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
    }
}
