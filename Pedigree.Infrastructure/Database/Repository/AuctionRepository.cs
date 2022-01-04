using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class AuctionRepository : GenericRepository<Auction>, IAuctionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;

        public AuctionRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<Auction> CreateAuction(Auction auction)
        {
            await _dbContext.Auction.AddAsync(auction);

            await _dbContext.SaveChangesAsync();

            return auction;
        }

        public async Task<AuctionDetail> CreateAuctionDetail(AuctionDetail auctionDetail)
        {
            await _dbContext.AuctionDetail.AddAsync(auctionDetail);
            await _dbContext.SaveChangesAsync();

            return auctionDetail;
        }

        public async Task<Auction[]> GetAuctions()
        {
            
            Auction[] auctions =  await _dbContext.Auction.ToArrayAsync();
            foreach(var auction in auctions)
            {
                DateTime date = DateTime.Today;
                DateTime dd = DateTime.Parse(auction.AuctionDate);
                if (dd >= date)
                {
                    var count = await _dbContext.AuctionDetail.Where(ad => ad.AuctionId == auction.Id).CountAsync();
                    auction.Count = count;
                }
                else
                {
                    Console.WriteLine(-1);
                }
            }
            return auctions;
        }

        public async Task DeleteAuction(int auctionId)
        {
            var auction = await _dbContext.Auction.FirstOrDefaultAsync(a => a.Id == auctionId);
            if(auction != null)
            {
                _dbContext.Auction.Remove(auction);
                await _dbContext.SaveChangesAsync();
            }
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(string.Format(Queries.Delete_AuctionDetails_ByAuctionId, auctionId));
            }  
        }

        public async Task DeleteAuctionDetail(int auctionDetailId)
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(string.Format(Queries.Delete_AuctionDetails_ById, auctionDetailId));
            }
            
        }

        public async Task<AuctionDetail[]> GetAuctionDetail(int auctionId)
        {
            AuctionDetail[] auctionDetails = await _dbContext.AuctionDetail.Where(ad => ad.AuctionId == auctionId).ToArrayAsync();
            foreach(var auctionDetail in auctionDetails)
            {
                Horse father = await GetHorseById(auctionDetail.SireId);
                Horse mother = await GetHorseById(auctionDetail.DamId);
                auctionDetail.FatherName = father.Name;
                auctionDetail.MotherName = mother.Name;
                auctionDetail.mtDNAHap = await GetMtDNAHap(mother.Name);
                Horse horse = await _dbContext.Horses.Where(h => h.Name == auctionDetail.Name).FirstOrDefaultAsync();
                if (horse != null) auctionDetail.HorseId = horse.Id;
            }
            
            return auctionDetails;
        }

        public async Task<Horse> GetHorseById(int horseId)
        {
            return await _dbContext.Horses.AsNoTracking().FirstOrDefaultAsync(h => h.Id == horseId);
        }

        public async Task<Auction> GetAuction(int auctionId)
        {
            Auction auction = await _dbContext.Auction.Where(a => a.Id == auctionId).FirstOrDefaultAsync();
            auction.Count = await _dbContext.AuctionDetail.Where(ad => ad.AuctionId == auctionId).CountAsync();

            return auction;
        }

        public async Task<MtDNAHap> GetMtDNAHap(string motherName)
        {
            HaploType DNAHap = null;
            Horse horse = await _dbContext.Horses.Where(h => h.Name == motherName).FirstOrDefaultAsync();
            DNAHap = await _dbContext.HaploTypes.Where(ht => ht.Id == horse.MtDNA).FirstOrDefaultAsync();
            var mtDNGHapGroupId = DNAHap.GroupId;

            HaploGroup mtDNAHapGroup = await _dbContext.HaploGroups.Where(hg => hg.Id == mtDNGHapGroupId).FirstOrDefaultAsync();
            string mtDNAHapGroupTitle = mtDNAHapGroup.Title;
            MtDNAHap mtDNAHap = new MtDNAHap
            {
                Name = mtDNAHapGroupTitle + "-" + DNAHap.Name,
                Color = mtDNAHapGroup.Color
            };

            return mtDNAHap;
        }
    }
}
