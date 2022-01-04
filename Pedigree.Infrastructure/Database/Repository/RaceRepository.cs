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
    public class RaceRepository : GenericRepository<Race>, IRaceRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public RaceRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<EntitiesWithTotal<Race>> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            if (page > 1)
            {
                return await SearchPartialPaginatedWithTotalAsync(q, page, sort, direction, size);
            }

            IEnumerable<Race> races;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            if (!sort.Equals("country")) sorting += ", Country ASC";
            var query = Queries.Get_Races_Paged_Query(q, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    races = multi.Read<Race>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Race>
            {
                Data = races.Distinct(),
                Total = rowCount
            };
        }

        private async Task<EntitiesWithTotal<Race>> SearchPartialPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            IEnumerable<Race> races;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query = Queries.Get_Races_Paged_Query(q, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    races = multi.Read<Race>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Race>
            {
                Data = races,
                Total = rowCount
            };
        }

        public async Task<bool> HasResult(int raceId)
        {
            return await _dbContext.Positions.AnyAsync(s => s.RaceId == raceId);
        }

        public async Task<Race> GetRace(string name, DateTime date, string country)
        {
            return await _dbContext.Races.AsNoTracking().FirstOrDefaultAsync(r => r.Name == name && r.Date == date && r.Country == country);
        }

        public async Task<IEnumerable<HorseRace>> GetRacesForHorse(string oId, string sort, string order)
        {
            if (sort == null) sort = "Place";
            if (order == null) order = "ASC";

            var sorting = string.Format("{0} {1}", sort, order);
            var query = string.Format(Queries.Horse_Races_Query, oId, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var races = await conn.QueryAsync<HorseRace>(query);
                return races;
            }

        }

        public async Task<IEnumerable<HorseRace>> GetRacesForHorses(string[] oIds)
        {
            var query = string.Format(Queries.Horses_Races_Query, $"'{string.Join("','", oIds)}'");
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var races = await conn.QueryAsync<HorseRace>(query);
                return races;
            }

        }

        public async Task<IEnumerable<HorseRace>> GetRacesForStakeWinners()
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var races = await conn.QueryAsync<HorseRace>(Queries.Horse_Races_SW);
                return races;
            }

        }

        public async Task<IEnumerable<Race>> GetAllRaces()
        {
            IEnumerable<Race> races;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                races = await conn.QueryAsync<Race>(Queries.Get_All_Races);
            }

            return races;
        }

        public async Task<IEnumerable<Race>> GetHistRaces()
        {
            IEnumerable<Race> races;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                races = await conn.QueryAsync<Race>(Queries.Get_Hist_Races);
            }

            return races;
        }

        public double GetStallionAvgRunnersCount(string oId)
        {
            double result;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = string.Format(Queries.Get_Stallion_Avg_Runners_Count, oId);
                result = conn.Query<double>(query).First();
            }

            return result;
        }

        public int GetStallionWinnersCount(string oId)
        {
            int result;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = string.Format(Queries.Get_Stallion_Winners_Count, oId);
                result = conn.Query<int>(query).First();
            }

            return result;
        }

        public int GetStallionRunnersCount(string oId)
        {
            int result;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = string.Format(Queries.Get_Stallion_Runners_Count, oId);
                result = conn.Query<int>(query).First();
            }

            return result;
        }

        public async Task<double> GetExpectedStallionWnrsValue()
        {
            double result;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = conn.Query<double>(Queries.Get_Expected_Stallion_Wnrs_Value).First();
            }

            return result;
        }
    }
}
