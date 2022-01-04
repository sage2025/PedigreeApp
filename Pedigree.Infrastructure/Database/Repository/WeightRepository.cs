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
    public class WeightRepository : GenericRepository<Weight>, IWeightRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public WeightRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<EntitiesWithTotal<Weight>> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            if (page > 1)
            {
                return await SearchPartialPaginatedWithTotalAsync(q, page, sort, direction, size);
            }

            IEnumerable<Weight> weights;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query = Queries.Get_Weights_Paged_Query(q, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    weights = multi.Read<Weight>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Weight>
            {
                Data = weights.Distinct(),
                Total = rowCount
            };
        }

        private async Task<EntitiesWithTotal<Weight>> SearchPartialPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            IEnumerable<Weight> weights;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query = Queries.Get_Weights_Paged_Query(q, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    weights = multi.Read<Weight>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new EntitiesWithTotal<Weight>
            {
                Data = weights,
                Total = rowCount
            };
        }
    }
}
