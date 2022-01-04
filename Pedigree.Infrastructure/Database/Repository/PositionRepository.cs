using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
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
    public class PositionRepository : GenericRepository<Position>, IPositionRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public PositionRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<EntitiesWithTotal<Position>> SearchPaginatedWithTotalAsync(int raceId, string q, int page, int size)
        {
            if (q != null) q = q.Replace("'", "''");
            if (page > 1)
            {
                return await SearchPartialPaginatedWithTotalAsync(raceId, q, page, size);
            }

            IEnumerable<Position> positions;
            var rowCount = 0;
            var query = Queries.Get_Positions_Paged_Query(raceId, q);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    positions = multi.Read<Position>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Position>
            {
                Data = positions.Distinct(),
                Total = rowCount
            };
        }

        private async Task<EntitiesWithTotal<Position>> SearchPartialPaginatedWithTotalAsync(int raceId, string q, int page, int size)
        {
            if (q != null) q = q.Replace("'", "''");
            IEnumerable<Position> positions;
            var rowCount = 0;
            var query = Queries.Get_Positions_Paged_Query(raceId, q);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    positions = multi.Read<Position>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Position>
            {
                Data = positions,
                Total = rowCount
            };
        }

        public async Task<Position> GetPosition(string horseOId, int raceId)
        {
            return await _dbContext.Positions.AsNoTracking().FirstOrDefaultAsync(p => p.HorseOId == horseOId && p.RaceId == raceId);
        }

        public async Task<bool> HasRaceResult(string horseOId)
        {
            return await _dbContext.Positions.AnyAsync(s => s.HorseOId == horseOId);
        }

        public async Task<IEnumerable<Position>> GetRacePositions(int raceId)
        {
            return await _dbContext.Positions.Where(x => x.RaceId == raceId).ToListAsync();
        }

        public async Task<IEnumerable<Position>> GetAllPositions()
        {
            IEnumerable<Position> positions;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                positions = await conn.QueryAsync<Position>(Queries.Get_All_Positions);
            }

            return positions;
        }
    }
}
