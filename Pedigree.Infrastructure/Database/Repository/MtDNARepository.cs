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
    public class MtDNARepository : GenericRepository<HaploGroup>, IMtDNARepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public MtDNARepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<int> CreateHaploType(HaploType haploType)
        {
            await _dbContext.HaploTypes.AddAsync(haploType);

            await _dbContext.SaveChangesAsync();

            return haploType.Id;
        }

        public async Task DeleteHaploType(int typeId)
        {
            var haploType = await _dbContext.HaploTypes.FirstOrDefaultAsync(t => t.Id == typeId);
            if (haploType != null)
            {
                _dbContext.HaploTypes.Remove(haploType);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<HaploGroup[]> GetHaploGroups()
        {
            return await _dbContext.HaploGroups.Include(x => x.Types).ToArrayAsync();
        }

        public Task UpdateHaploType(HaploType haploType)
        {
            throw new NotImplementedException();
        }

        public async Task<int> CreateMtDNAFlag(MtDNAFlag mtDNAFlag)
        {
            await _dbContext.MtDNAFlags.AddAsync(mtDNAFlag);

            await _dbContext.SaveChangesAsync();

            return mtDNAFlag.Id;
        }

        public async Task<MtDNAFlag> GetMtDNAFlag(int flagId)
        {
            return await _dbContext.MtDNAFlags.FirstOrDefaultAsync(x => x.Id == flagId);
        }

        public async Task<MtDNAFlag[]> GetMtDNAFlags()
        {
            MtDNAFlag[] result;

            var query = Queries.Get_MtDNA_Flags;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = conn.Query<MtDNAFlag>(query).ToArray();
            }

            return result;
        }

        public async Task DeleteMtDNAFlag(int flagId)
        {
            var mtDNAFlag = await _dbContext.MtDNAFlags.FirstOrDefaultAsync(t => t.Id == flagId);
            if (mtDNAFlag != null)
            {
                _dbContext.MtDNAFlags.Remove(mtDNAFlag);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
