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
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class RelationshipRepository : GenericRepository<Relationship>, IRelationshipRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public RelationshipRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }

        public async Task<bool> DeleteRelationshipByOIdComoboAsync(string horseOid, string parentOid)
        {
            try
            {
                var relationship = await _dbContext.Relationships.FirstOrDefaultAsync(f => f.HorseOId == horseOid && f.ParentOId == parentOid);
                _dbContext.Relationships.Remove(relationship);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteRelationshipByParentTypeAsync(string horseOId, string parentType)
        {
            try
            {
                var relationship = await _dbContext.Relationships.FirstOrDefaultAsync(f => f.HorseOId == horseOId && f.ParentType == parentType);
                _dbContext.Relationships.Remove(relationship);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Relationship>> GetImmediateParents(string horseOid)
        {

            return await _dbContext.Relationships.Where(h => h.HorseOId == horseOid).ToListAsync();
        }

        public async Task<Relationship> GetRelationshipByOIdComoboAsync(string horseOid, string parentType)
        {
            var relationship = await _dbContext.Relationships.AsNoTracking().FirstOrDefaultAsync(r => r.HorseOId == horseOid && r.ParentType == parentType);
            return relationship;
        }


        public async Task<HorsesWithTotal> GetHorseFemaleTailAsync(int id)
        {
            var result = new HorsesWithTotal();
            var query = string.Format(Queries.Get_Female_Tail_By_Horse, id);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result.Horses = await conn.QueryAsync<Horse>(query);
            }
            return result;
        }

        public async Task<HorsesWithTotal> GetUniqueAncestorsAsync(int horseId, int maxGen)
        {
            var result = new HorsesWithTotal();
            var query = string.Format(Queries.Get_Unique_Ancestors_By_Horse, horseId, maxGen);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result.Horses = await conn.QueryAsync<Horse>(query);
            }
            return result;
        }

        public async Task<IEnumerable<Inbreed>> GetInbreedingsAsync(string horseOId, string[] oids)
        {
            var query = string.Format(Queries.Get_Inbreedings_By_Horse, horseOId, string.Join("','", oids));
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                return await conn.QueryAsync<Inbreed>(query);
            }
        }

        public async Task<HorsesWithTotal> GetHorseOffspringsAsync(int id)
        {
            var result = new HorsesWithTotal();

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result.Horses = await conn.QueryAsync<Horse>(Queries.Get_Offspring_By_Horse, new { Id = id });
            }

            return result;
        }

        public async Task<HorsesWithTotal> GetHorseSibilingsAsync(int id, string parentType = "Mother")
        {
            var result = new HorsesWithTotal();

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(Queries.Get_Sibilings_Horse_From_Same_Parent_Sex, new { ParentType = parentType, Id = id }))
                {
                    result.Horses = multi.Read<Horse>();
                }
            }
            return result;
        }

        public async Task<IEnumerable<Horse>> GetChildrenHorses(string parentOId)
        {
            return await (
                    from r in _dbContext.Relationships
                    from h in _dbContext.Horses.Where(h => h.OId == r.HorseOId).DefaultIfEmpty()
                    where !h.isDeleted && r.ParentOId == parentOId
                    select h
            ).ToListAsync();
        }

        public async Task<HorsesWithTotal> GetGrandparents(string sort, string order, int page, int size)
        {
            IEnumerable<Horse> horses;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, order);
            var query = string.Format(Queries.Get_Grandparents_Paged_Query, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    horses = multi.Read<Horse>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new HorsesWithTotal
            {
                Horses = horses,
                Total = rowCount
            };
        }

        public async Task<HorsesWithTotal> GetSWOffsprings(int maleHorseId, int femaleHorseId)
        {
            var result = new HorsesWithTotal();

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result.Horses = await conn.QueryAsync<Horse>(Queries.Get_SW_Offspring_By_Horse, new { MaleId = maleHorseId, FemaleId = femaleHorseId });
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetSWOffspringsBySire(int maleHorseId)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(Queries.Get_SW_Offsprings_By_Sire, new { Id = maleHorseId }, commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetSWOffspringByBroodmareSire(int maleHorseId)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(Queries.Get_SW_Offsprings_By_BroodmareSire, new { Id = maleHorseId }, commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetStakeWinnersBySireDescendants(int maleHorseId, int gen = 5)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(string.Format(Queries.Get_Stake_Winners_By_Sire_Descendants, maleHorseId, gen), commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetStakeWinnersByBroodmareSireDescendants(int maleHorseId, int gen = 5)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(string.Format(Queries.Get_Stake_Winners_By_BroodmareSire_Descendants, maleHorseId, gen), commandTimeout: 1000);
            }

            return result;
        }

        public async Task<Horse> GetParent(int horseId, string parentType)
        {
            Horse result = null;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = string.Format(Queries.Get_Parent_By_Type, parentType, horseId);
                result = await conn.QueryFirstAsync<Horse>(query);
            }
            return result;
        }

        public async Task<IEnumerable<Horse>> GetStakeWinnersByWildcard1Search(int horse1Id, int horse2Id)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(string.Format(Queries.Get_Stake_Winners_By_Wildcard1, horse1Id, horse2Id), commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetStakeWinnersByWildcard2Search(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = Queries.Get_Stake_Winners_By_Wildcard2(horse1Id, horse2Id, horse3Id, horse4Id);
                result = await conn.QueryAsync<Horse>(query, commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<Horse>> GetStakeWinnersByWildcardQueryByPosition(Dictionary<int, int> searches)
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                var query = Queries.Get_Stake_Winners_By_Wildcard_Query_Position(searches);
                result = await conn.QueryAsync<Horse>(query, commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<HorseHeirarchy>> GetStakeWinnersByMareDescendants(int femaleId, int gen = 5)
        {
            IEnumerable<HorseHeirarchy> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(string.Format(Queries.Get_Stake_Winners_By_Mare_Descendants, femaleId, gen), commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<HorseHeirarchy>> GetHorsesForFemaleLineSearch(int femaleId)
        {
            IEnumerable<HorseHeirarchy> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(string.Format(Queries.Get_Horses_For_Female_Line, femaleId), commandTimeout: 1000);
            }

            return result;
        }

        public async Task<IEnumerable<HorseMtDNALookupDTO>> GetHorsesForMtDNALookup(int haploGroupId)
        {
            IEnumerable<HorseMtDNALookupDTO> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseMtDNALookupDTO>(string.Format(Queries.Get_Horses_For_MtDNA_Lookup, haploGroupId), commandTimeout: 1000);
            }

            return result;
        }
    }
}
