using System;
using System.Collections.Generic;
using System.Text;

namespace Pedigree.Core.Data.DTO
{
    public class AuctionDTO
    {
        public int Id { get; set; }
        public string AuctionDate { get; set; }
        public string AuctionName { get; set; }
        
        public string AuctionType { get; set; }
        public int Count { get; set; }
    }
}
