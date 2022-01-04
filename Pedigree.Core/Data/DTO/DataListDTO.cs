using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class DataListDTO <T>
    {
        public int Total { get; set; }
        public IEnumerable<T> Data { get; set; }

    }
}
