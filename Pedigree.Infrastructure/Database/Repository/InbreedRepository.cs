using Dapper;
using Microsoft.Data.SqlClient;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class InbreedRepository : GenericRepository<Inbreed>, IInbreedRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public InbreedRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task RemoveUnnecessaryInbreeds()
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString.Value))
                {
                    await conn.QueryAsync(Queries.Remove_Unnecessary_Inbreeds);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
