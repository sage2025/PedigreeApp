using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IAuctionRepository : IGenericRepository<Auction>
    {
        Task<Auction> CreateAuction(Auction auction);
        Task<AuctionDetail> CreateAuctionDetail(AuctionDetail auctionDetail);
        Task<Auction[]> GetAuctions();
        Task DeleteAuction(int auctionId);
        Task DeleteAuctionDetail(int auctionDetailId);
        Task<AuctionDetail[]> GetAuctionDetail(int auctionId);
        Task<Auction> GetAuction(int auctionId);
        Task<MtDNAHap> GetMtDNAHap(string motherName);
    }
}
