using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.Entity.InMemoryOnly
{
    public class PedItem
    {
        public int HorseId { get; set; }
        public int FatherId { get; set; }
        public int MotherId { get; set; }

        public int Year { get; set; }
        public int Sex { get; set; }

        public PedItem(int horseId)
        {
            HorseId = horseId;
        }   
    }
}
