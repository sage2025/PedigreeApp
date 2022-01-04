using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IAuctionService
    {
        Task<Auction> AddAuction(string date, string name);
        Task<SaleSaveHorseResponseDTO> AddHorse(int auctionId, int number, string name, string type, int yob, string sex, string country, string fatherName, string motherName);
        Task<bool> AddAuctionDetails(AuctionDetailDTO[] details);
        Task<Auction[]> GetAuctions();
        Task DeleteAuction(int auctionId);
        Task DeleteAuctionDetail(int auctionDetailId);
        Task<double> CheckPedigComp(int horseId);
        Task<AuctionDetail[]> GetAuctionDetail(int auctionId);
        Task<Auction> GetAuction(int auctionId);
        Task<MtDNAHap> GetMtDNAHap(string motherName);
    }
}
