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
    public class MLRepository : GenericRepository<MLModel>, IMLRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public MLRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<int> CreateMLModel(MLModel mlModel)
        {
            await _dbContext.MLModels.AddAsync(mlModel);

            await _dbContext.SaveChangesAsync();

            return mlModel.Id;
        }
       
        public async Task<IEnumerable<MLModelData>> GetMLModelData()
        {
            IEnumerable<MLModelData> result = null;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<MLModelData>(Queries.Get_ML_Model_Data, commandTimeout:1000);
            }
            return result;
        }

        public async Task<MLModel> GetLastMLModel()
        {
            return await _dbContext.MLModels.Where(x => x.Deployed).OrderByDescending(x => x.CreatedAt).Take(1).FirstOrDefaultAsync();
        }
    }
}
