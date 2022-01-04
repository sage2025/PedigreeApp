using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using System.Threading.Tasks;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System.Linq.Dynamic.Core;
using System.Data.Entity.SqlServer;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Service;
using System.Text.Json;

namespace Pedigree.Infrastructure.Database.Repository
{
    public class HorseRepository : GenericRepository<Horse>, IHorseRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ConnectionString _connectionString;
        public HorseRepository(ApplicationDbContext context, ConnectionString connectionString)
          : base(context)
        {
            _dbContext = context;
            _connectionString = connectionString;

        }


        public async Task<bool> DuplicateCheck(string name, int age, string sex, string country)
        {
            try
            {
                var result = -1;
                using (var conn = new SqlConnection(_connectionString.Value))
                {
                    result = (await conn.QueryAsync<int>(Queries.Check_Dupliacte_Horse_By_Params, new { Name = name, Age = age, Sex = sex, Country = country })).FirstOrDefault();
                    if (result > 0)
                    {
                        return true;
                    }
                }
            }
            catch
            {
                throw;
            }
            return false;
        }

        public async Task<Horse> GetByOid(string Oid)
        {
            return await _dbContext.Horses.AsNoTracking().FirstOrDefaultAsync(h => h.OId == Oid);
        }

        public async Task<bool> HasChildren(string oid)
        {
            return await _dbContext.Relationships.AnyAsync(s => s.ParentOId == oid);
        }

        public async Task<IEnumerable<int>> GetHorseIdsForCoefficient()
        {
            DateTime oldDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));  // A week ago
            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && h.Age >= 1990 && (c.COIUpdatedAt == null || c.COIUpdatedAt < oldDate)
                    select h
            ).Select(h => h.Id).Distinct().Take(300).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetHorseIdsForProbOrig(int limit)
        {
            DateTime oldDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));  // A week ago
            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    from p in _dbContext.Pedigs.Where(p => p.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && c.ZHistoricalBPR != null && c.Pedigcomp >= 95 && (p.ProbOrigsUpdatedAt == null || p.ProbOrigsUpdatedAt < oldDate)
                    select h
            ).Select(h => h.Id).Distinct().Take(limit).ToListAsync();
        }
        public async Task<int> GetHorsesCountForProbOrig()
        {
            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    from p in _dbContext.Pedigs.Where(p => p.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && c.ZHistoricalBPR != null && c.Pedigcomp >= 95
                    select h
            ).Select(h => h.Id).Distinct().CountAsync();
        }

        public async Task<IEnumerable<int>> GetHorseIdsForGRain(int limit)
        {
            DateTime halfMonthAgo = DateTime.Now.Subtract(new TimeSpan(15, 0, 0, 0, 0));  // 15 days ago
            DateTime oneDayAgo = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));  // One day ago

            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && c.ZCurrentBPR != null && c.Pedigcomp >= 95 && (c.GRainProcessStartedAt == null || c.GRainProcessStartedAt < oneDayAgo) && (c.GRainUpdatedAt == null || c.GRainUpdatedAt < halfMonthAgo)
                    select h
            ).Select(h => h.Id).Distinct().Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetHorseIdsForUniqueAncestorsCount(int limit)
        {
            DateTime aWeekAgo = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));  // One week ago
            DateTime oneDayAgo = DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0, 0));  // One day ago

            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && c.ZCurrentBPR != null && (c.UniqueAncestorsCountUpdatedAt == null || c.UniqueAncestorsCountUpdatedAt < aWeekAgo)
                    select h
            ).Select(h => h.Id).Distinct().Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<int>> GetHorseIdsForPedigree(int limit)
        {
            DateTime aWeekAgo = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0, 0));  // A week ago
            return await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Positions.Where(c => c.HorseOId == h.OId).DefaultIfEmpty()
                    from p in _dbContext.Pedigs.Where(p => p.HorseOId == h.OId).DefaultIfEmpty()
                    where !h.isDeleted && c.Place == 1 && (p.PedigreeUpdatedAt == null || p.PedigreeUpdatedAt < aWeekAgo)
                    select h
            ).Select(h => h.Id).Distinct().Take(limit).ToListAsync();
        }

        public async Task<IEnumerable<Horse>> GetUniqueAncestors(int horseId, int gen=10)
        {
            IEnumerable<Horse> result = null;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(Queries.Get_Unique_Ancestors, new { HorseId=horseId, Gen=gen, YOB=0});
            }
            return result;
        }

        public async Task<int> GetUniqueAncestorsCount(int horseId, int gen)
        {
            IEnumerable<Horse> result = await GetUniqueAncestors(horseId, gen);
            return result.Count();
        }


        public async Task<Horse> GetHorseById(int id)
        {
            Horse result = null;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryFirstOrDefaultAsync<Horse>(Queries.Get_Horse_By_Id, new { Id = id});
            }
            return result;
        }

        public async Task<HorseWithParentChildren> GetHorseWithParentChildrenById(int id)
        {
            var horse = await _dbContext.Horses.FirstOrDefaultAsync(h => h.Id == id);

            if (horse.MtDNA != null) {
                var mtDNA = await _dbContext.HaploTypes.Include(x => x.Group).FirstOrDefaultAsync(ht => ht.Id == horse.MtDNA);
                horse.MtDNATitle = mtDNA.Group.Title + "-" + mtDNA.Name;
                horse.MtDNAColor = mtDNA.Group.Color;
            }
            var parentHorses = await GetParents(horse.OId);

            var childrenHorses = await GetChildren(horse.OId);

            return new HorseWithParentChildren
            {

                MainHorse = horse,
                Parents = parentHorses,
                Children = childrenHorses
            };
        }

        public async Task<bool> SoftDelete(int id)
        {
            try
            {
                var horse = await _dbContext.Horses.FirstOrDefaultAsync(h => h.Id == id);
                horse.isDeleted = true;
                await Update(id, horse);
                return true;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public async Task<HorseWithParentChildren> GetHorseWithParentChildrenByOId(string Oid)
        {
            var horse = await _dbContext.Horses.FirstOrDefaultAsync(h => h.OId == Oid);

            var parentHorses = await GetParents(horse.OId);

            var childrenHorses = await GetChildren(horse.OId);

            return new HorseWithParentChildren
            {

                MainHorse = horse,
                Parents = parentHorses,
                Children = childrenHorses
            };
        }

        public HorsesWithTotal GetPaginatedWithTotal(int page, string sort, string direction, int size)
        {
            var sorting = string.Format("{0} {1}", sort, direction);
            var horses = (from h in _dbContext.Set<Horse>()
                          where h.isDeleted == false
                          select new Horse
                          {
                              Id = h.Id,
                              Name = h.Name,
                              Sex = h.Sex,
                              Age = h.Age,
                              FatherName = h.FatherName,
                              MotherName = h.MotherName,
                              Country = h.Country,
                              Family = h.Family,
                              OId = h.OId
                          }).OrderBy(sorting).GetPagedHorses(page, size);

            return new HorsesWithTotal
            {
                Horses = horses.Results,
                Total = horses.RowCount
            };
        }

        public HorsesWithTotal SearchNameStartsWithPaginatedWithTotal(string q, int page, int size)
        {
            q = q.Replace("'", "''");
            var horses = (from h in _dbContext.Set<Horse>()
                          where h.Name == q && h.isDeleted == false
                          orderby h.Age descending
                          select new Horse
                          {
                              Id = h.Id,
                              Name = h.Name,
                              Sex = h.Sex,
                              Age = h.Age,
                              Country = h.Country,
                              Family = h.Family,
                              OId = h.OId
                          }).GetPagedHorses(page, size);

            return new HorsesWithTotal
            {
                Horses = horses.Results,
                Total = horses.RowCount
            };
        }

        public HorsesWithTotal SearchPaginatedWithTotal(string q, int page, string sort, string direction, int size)
        {
            var result = new HorsesWithTotal();
            q = q.Replace("'", "''");

            var sorting = string.Format("{0} {1}", sort, direction);

            var query1 = string.Format(Queries.Search_Exact_Match_Horses_Paged_Query, q, sorting);
            var query2 = string.Format(Queries.GET_COUNT_HORSES_EXACT_MATCH_Query, q);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = conn.QueryMultiple(string.Format("{0}{1}", query1, query2), new { PageNumber = page, PageSize = size }))
                {
                    result.Horses = multi.Read<Horse>();
                    result.Total = multi.Read<int>().FirstOrDefault();
                }
            }
            return result;
        }

        public async Task<HorsesWithTotal> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            if (page > 1)
            {
                return await SearchPartialPaginatedWithTotalAsync(q, page, sort, direction, size);
            }

            IEnumerable<Horse> horses;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query1 = string.Format(Queries.Search_Exact_Match_Horses_Paged_Query, q, sorting);
            var query2 = string.Format(Queries.Get_Horses_Paged_Query, q, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(string.Format("{0}{1}", query1, query2), new { PageNumber = page, PageSize = size }))
                {
                    horses = multi.Read<Horse>();
                    var matchingHorses = multi.Read<Horse>();
                    if (matchingHorses != null)
                    {
                        horses = horses.Union(matchingHorses, new HorseEqComparer());
                    }
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            return new HorsesWithTotal
            {
                Horses = horses.Distinct(),
                Total = rowCount
            };
        }

        private async Task<HorsesWithTotal> SearchPartialPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            IEnumerable<Horse> horses;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query = string.Format(Queries.Get_Horses_Paged_Query, q, sorting);
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

        public async Task<HorsesWithTotal> SearchWithGenderPaginatedWithTotalAsync(string q, string gender, int page, string sort, string direction, int size)
        {
            q = q.Replace("'", "''");
            IEnumerable<Horse> horses;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, direction);
            var query = string.Format(Queries.Search_Horses_With_Gender_Paged_Query, q, gender, sorting);
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
        public async Task<IEnumerable<HorseHeirarchy>> GetHorseHeirarchyBottomUp(int id, int generationLevel, string SD)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_Heirarchy_Bottom_Up_Query, id, generationLevel, SD == null ? "S" : SD, SD == null ? "D" : SD);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query);
            }
            return result;
        }

        public async Task<IEnumerable<Horse>> GetParentsOrChildren(string oId, bool needParents)
        {
            if (needParents)
            {
                return await GetParents(oId);
            }
            else
            {
                return await GetChildren(oId);
            }
        }

        public async Task<HorsePedigree> GetPedigree(int horseId, int gen, int yob)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_Pedigree_Query, horseId, gen, yob);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query, new { HorseId=horseId, Gen=gen, YOB=yob }, commandTimeout:1000);
            }

            return new HorsePedigree(horseId, result.ToList());
        }

        public async Task<HorsePedigree> GetPedigreeComp(int horseId, int gen, int yob)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_PedigreeComp_Query, horseId, gen, yob);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query, commandTimeout: 1000);
            }

            return new HorsePedigree(horseId, result.ToList());
        }

        public async Task<HorsePedigree> GetHypotheticalPedigree(int sireId, int damId, int gen, int yob)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_Hypothetical_Pedigree_Query, sireId, damId, gen, yob);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query, new { SireId = sireId, DamId = damId, Gen = gen, YOB = yob }, commandTimeout: 1000);
            }

            return new HorsePedigree(1, result.ToList());
        }

        public async Task<HorsePedigree> GetFullPedigree(int horseId, int gen, int yob)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_Full_Pedigree_Query, horseId, gen, yob);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query, new { HorseId = horseId, Gen = gen, YOB = yob }, commandTimeout: 1000);
            }

            return new HorsePedigree(horseId, result.ToList());
        }

        public async Task<HorsePedigree> GetHypotheticalFullPedigree(int sireId, int damId, int gen, int yob)
        {
            IEnumerable<HorseHeirarchy> result = null;

            var query = string.Format(Queries.Horse_Hypothetical_Full_Pedigree_Query, sireId, damId, gen, yob);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<HorseHeirarchy>(query, new { SireId = sireId, DamId = damId, Gen = gen, YOB = yob }, commandTimeout: 1000);
            }

            return new HorsePedigree(1, result.ToList());
        }

        public async Task<HorsesWithTotal> GetIncompletedPedigreeHorses(int year, string sort, string order, int page, int size)
        {

            IEnumerable<Horse> horses;
            var rowCount = 0;
            var sorting = string.Format("{0} {1}", sort, order);
            var query = string.Format(Queries.Incompleted_Pedigree_Horses_Paged_Query, year, sorting);
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

        public async Task<IEnumerable<HorseTwin>> GetTwinDams()
        {

            IEnumerable<HorseTwin> dams;
            var query = Queries.Get_Twins_Dam_List_Query;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                dams = await conn.QueryAsync<HorseTwin>(query);
            }
            return dams;
        }

        public async Task<HorsesWithTotal> GetFounders()
        {

            IEnumerable<Horse> horses;
            var query = string.Format(Queries.Get_Founder_List_Query);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                horses = await conn.QueryAsync<Horse>(query);
            }

            return new HorsesWithTotal
            {
                Horses = horses,
                Total = horses.Count()
            };
        }

        public async Task<HorsesWithTotal> GetInbreedingHorses(string oid, string sort, string order, int page, int size)
        {
            IEnumerable<Horse> horses;
            var rowCount = 0;

            var sorting = string.Format("{0} {1}", sort, order);
            var query = string.Format(Queries.Get_Inbreeding_Horses_Paged_Query, oid, sorting);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                using (var multi = await conn.QueryMultipleAsync(query, new { PageNumber = page, PageSize = size }))
                {
                    horses = multi.Read<Horse>();
                    rowCount = multi.Read<int>().FirstOrDefault();
                }
            }

            await AttatchParents(horses);
            return new HorsesWithTotal
            {
                Horses = horses,
                Total = rowCount
            };
        }

        public async Task<Coefficient> GetCoefficient(string oid)
        {
            return await _dbContext.Coefficients.FirstOrDefaultAsync(c => c.HorseOId == oid);
        }

        public async Task<Coefficient> GetCoefficientByHorseId(int horseId)
        {
            return await (from h in _dbContext.Horses
                   join c in _dbContext.Coefficients
                   on h.OId equals c.HorseOId
                   where h.Id == horseId
                   select c).FirstOrDefaultAsync();
        }

        public async Task UpdateCoefficient(Coefficient coefficient)
        {
            _dbContext.Coefficients.Update(coefficient);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> CreateCoefficient(Coefficient coefficient)
        {
            _dbContext.Coefficients.Add(coefficient);
            return await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateFamilyForTailFemale(int horseId, string family)
        {
            var query = string.Format(Queries.Update_Family_For_Tail_Female, horseId, family != null ? $"'{family}'" : "NULL");
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task UpdateMtDNAForTailFemale(int horseId, int mtDNA)
        {

            var query = string.Format(Queries.Update_MtDNA_For_Tail_Female, horseId, mtDNA == -1 ? "NULL" : mtDNA.ToString());
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task<IEnumerable<Horse>> AttatchParents(IEnumerable<Horse> horses)
        {
            try
            {
                var query = string.Format(Queries.Attach_Parents_Query, string.Join(",", horses.Select(h => h.Id).ToArray()));
                using (var conn = new SqlConnection(_connectionString.Value))
                {
                    var horsesWithParents = await conn.QueryAsync<Horse>(query);

                    foreach (var horse in horses)
                    {
                        var p = horsesWithParents.FirstOrDefault(h => h.Id == horse.Id);
                        horse.FatherId = p.FatherId;
                        horse.FatherOId = p.FatherOId;
                        horse.FatherName = p.FatherName;
                        horse.MotherId = p.MotherId;
                        horse.MotherOId = p.MotherOId;
                        horse.MotherName = p.MotherName;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            return horses;
        }
        public async Task<IEnumerable<Horse>> GetHorsesByHaploTypes(int[] types)
        {
            IEnumerable<Horse> result = null;

            var query = string.Format(Queries.Horses_By_HaploTypes, string.Join(",", types));
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(query);
            }
            return result;
        }
        public async Task<IEnumerable<Horse>> GetModernHorses()
        {
            IEnumerable<Horse> result = null;

            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryAsync<Horse>(Queries.Mordern_Horses);
            }
            return result;
        }

        public async Task<IEnumerable<Horse>> GetCheckedFounders()
        {
            return await _dbContext.Horses.Where(x => x.IsFounder).ToListAsync();
        }

        public async Task CalculateCOIDs()
        {
            var query = Queries.Calculate_COID_Query;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query, null, null, 600);
            }
        }
        public async Task CalculateCOID(string oId)
        {
            var query = string.Format(Queries.Calculate_COID_For_Horse_Query, oId);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task<Pedig> GetPedigByHorseId(int horseId)
        {
            return await (
                    from p in _dbContext.Pedigs
                    from h in _dbContext.Horses.Where(h => h.OId == p.HorseOId)
                    where h.Id == horseId
                    select p
            ).FirstOrDefaultAsync();
        }

        public async Task<Pedig> GetPedig(string oid)
        {
            return await _dbContext.Pedigs.Where(c => c.HorseOId == oid).FirstOrDefaultAsync();
        }
        public async Task<bool> ExistPedig(string oid)
        {
            return await _dbContext.Pedigs.AnyAsync(c => c.HorseOId == oid);
        }
        public async Task UpdatePedig(Pedig pedig)
        {
            _dbContext.Pedigs.Update(pedig);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> CreatePedig(Pedig pedig)
        {
            _dbContext.Pedigs.Add(pedig);
            return await _dbContext.SaveChangesAsync();
        }
        public async Task<IEnumerable<Pedig>> GetPedigsForProbOrigs()
        {
            var pedigs = await (
                    from p in _dbContext.Pedigs
                    where p.ProbOrigsUpdatedAt != null && p.ProbOrigs != null
                    select new Pedig
                    {
                        Id = p.Id,
                        HorseOId = p.HorseOId,
                        ProbOrigs = p.ProbOrigs,
                        ProbOrigsUpdatedAt = p.ProbOrigsUpdatedAt
                    }
            ).ToListAsync();

            return pedigs;


            /*IEnumerable<Pedig> data;
            var query = string.Format(Queries.Get_Pedigs_For_ProbOrigs);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                data = await conn.QueryAsync<Pedig>(query);
            }
            return data;*/
        }

        public async Task UpdatePedigree(int horseId)
        {
            var query = string.Format(Queries.Update_Pedigree, horseId);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task DeleteProbOrigs()
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(Queries.Delete_ProbOrigs);
            }
        }

        public async Task DeleteGRainData()
        {
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(Queries.Delete_GRain_Data);
            }
        }

        public async Task UpdateProbOrigs(int horseId, List<ProbOrig> probOrigs)
        {
            var query = string.Format(Queries.Update_ProbOrigs, horseId, JsonSerializer.Serialize(probOrigs));
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task<IEnumerable<Horse>> GetEliteHorsesInHaploGroup(HaploGroupDTO g)
        {
            var horses = await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId)
                    where !h.isDeleted && c.ZHistoricalBPR >= 1 && (g.Title == "UNK" && h.MtDNA == null || g.Title != "UNK" && g.Types.Select(x => x.Id).ToList().Contains((int)(h.MtDNA!)))
                    select h
            ).ToListAsync();

            return horses;
        }
        public async Task<IEnumerable<Horse>> GetNonEliteHorsesInHaploGroup(HaploGroupDTO g)
        {
            var horses = await (
                    from h in _dbContext.Horses
                    from c in _dbContext.Coefficients.Where(c => c.HorseOId == h.OId)
                    where !h.isDeleted && c.ZHistoricalBPR <= -1 && c.ZHistoricalBPR >= -6 && (g.Title == "UNK" && h.MtDNA == null || g.Title != "UNK" && g.Types.Select(x => x.Id).ToList().Contains((int)(h.MtDNA!)))
                    select h
            ).ToListAsync();

            return horses;
        }

        public async Task<Ancestry> GetAncestry(string ancestorOId)
        {
            return await _dbContext.Ancestries.Where(a => a.AncestorOId == ancestorOId).FirstOrDefaultAsync();
        }
        public async Task UpdateAncestry(Ancestry ancestry)
        {
            _dbContext.Ancestries.Update(ancestry);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<int> CreateAncestry(Ancestry ancestry)
        {
            _dbContext.Ancestries.Add(ancestry);
            return await _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAncestry(Ancestry ancestry)
        {
            _dbContext.Ancestries.Remove(ancestry);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Ancestry>> GetAncestries(bool byQuery = true)
        {
            IEnumerable<Ancestry> data;
            if (byQuery)
            {
                var query = string.Format(Queries.Get_Ancestry_List_Query);
                using (var conn = new SqlConnection(_connectionString.Value))
                {
                    data = await conn.QueryAsync<Ancestry>(query);
                }
            } else
            {
                data = await _dbContext.Ancestries.AsNoTracking().ToListAsync();
            }
            return data;
        }

        public async Task<IEnumerable<PlotItem>> GetPopulationData()
        {

            var result = new List<PlotItem>();

            var allData = await _dbContext.Coefficients.Where(c => c.GRainUpdatedAt != null).ToListAsync();
            var eliteData = allData.Where(d => d.ZHistoricalBPR >= 1);
            var nonEliteData = allData.Where(d => d.ZHistoricalBPR <= -1 && d.ZHistoricalBPR >= -6);

            Calculator calc = new Calculator();


            /*** 1. Ballou ***/
            // All
            result.Add(new PlotItem
            {
                Category = "Ballou",
                Title = "All",
                Q1 = calc.Percentile(allData.Select(d => (double)d.Bal!).ToList(), 0.25),
                Median = calc.Percentile(allData.Select(d => (double)d.Bal!).ToList(), 0.5),
                Q3 = calc.Percentile(allData.Select(d => (double)d.Bal!).ToList(), 0.75),
                Min = allData.Select(d => (double)d.Bal!).Min(),
                Max = allData.Select(d => (double)d.Bal!).Max()
            });

            // Elite
            result.Add(new PlotItem
            {
                Category = "Ballou",
                Title = "Elite",
                Q1 = calc.Percentile(eliteData.Select(d => (double)d.Bal!).ToList(), 0.25),
                Median = calc.Percentile(eliteData.Select(d => (double)d.Bal!).ToList(), 0.5),
                Q3 = calc.Percentile(eliteData.Select(d => (double)d.Bal!).ToList(), 0.75),
                Min = eliteData.Select(d => (double)d.Bal!).Min(),
                Max = eliteData.Select(d => (double)d.Bal!).Max()
            });

            // Non-Elite
            result.Add(new PlotItem
            {
                Category = "Ballou",
                Title = "Non Elite",
                Q1 = calc.Percentile(nonEliteData.Select(d => (double)d.Bal!).ToList(), 0.25),
                Median = calc.Percentile(nonEliteData.Select(d => (double)d.Bal!).ToList(), 0.5),
                Q3 = calc.Percentile(nonEliteData.Select(d => (double)d.Bal!).ToList(), 0.75),
                Min = nonEliteData.Select(d => (double)d.Bal!).Min(),
                Max = nonEliteData.Select(d => (double)d.Bal!).Max()
            });

            /*** 2. AHC ***/
            // All
            result.Add(new PlotItem
            {
                Category = "AHC",
                Title = "All",
                Q1 = calc.Percentile(allData.Select(d => (double)d.AHC!).ToList(), 0.25),
                Median = calc.Percentile(allData.Select(d => (double)d.AHC!).ToList(), 0.5),
                Q3 = calc.Percentile(allData.Select(d => (double)d.AHC!).ToList(), 0.75),
                Min = allData.Select(d => (double)d.AHC!).Min(),
                Max = allData.Select(d => (double)d.AHC!).Max()
            });

            // Elite
            result.Add(new PlotItem
            {
                Category = "AHC",
                Title = "Elite",
                Q1 = calc.Percentile(eliteData.Select(d => (double)d.AHC!).ToList(), 0.25),
                Median = calc.Percentile(eliteData.Select(d => (double)d.AHC!).ToList(), 0.5),
                Q3 = calc.Percentile(eliteData.Select(d => (double)d.AHC!).ToList(), 0.75),
                Min = eliteData.Select(d => (double)d.AHC!).Min(),
                Max = eliteData.Select(d => (double)d.AHC!).Max()
            });

            // Non-Elite
            result.Add(new PlotItem
            {
                Category = "AHC",
                Title = "Non Elite",
                Q1 = calc.Percentile(nonEliteData.Select(d => (double)d.AHC!).ToList(), 0.25),
                Median = calc.Percentile(nonEliteData.Select(d => (double)d.AHC!).ToList(), 0.5),
                Q3 = calc.Percentile(nonEliteData.Select(d => (double)d.AHC!).ToList(), 0.75),
                Min = nonEliteData.Select(d => (double)d.AHC!).Min(),
                Max = nonEliteData.Select(d => (double)d.AHC!).Max()
            });

            /*** 3. Ballou ***/
            // All
            result.Add(new PlotItem
            {
                Category = "Kalinowski",
                Title = "All",
                Q1 = calc.Percentile(allData.Select(d => (double)d.Kal!).ToList(), 0.25),
                Median = calc.Percentile(allData.Select(d => (double)d.Kal!).ToList(), 0.5),
                Q3 = calc.Percentile(allData.Select(d => (double)d.Kal!).ToList(), 0.75),
                Min = allData.Select(d => (double)d.Kal!).Min(),
                Max = allData.Select(d => (double)d.Kal!).Max()
            });

            // Elite
            result.Add(new PlotItem
            {
                Category = "Kalinowski",
                Title = "Elite",
                Q1 = calc.Percentile(eliteData.Select(d => (double)d.Kal!).ToList(), 0.25),
                Median = calc.Percentile(eliteData.Select(d => (double)d.Kal!).ToList(), 0.5),
                Q3 = calc.Percentile(eliteData.Select(d => (double)d.Kal!).ToList(), 0.75),
                Min = eliteData.Select(d => (double)d.Kal!).Min(),
                Max = eliteData.Select(d => (double)d.Kal!).Max()
            });

            // Non-Elite
            result.Add(new PlotItem
            {
                Category = "Kalinowski",
                Title = "Non Elite",
                Q1 = calc.Percentile(nonEliteData.Select(d => (double)d.Kal!).ToList(), 0.25),
                Median = calc.Percentile(nonEliteData.Select(d => (double)d.Kal!).ToList(), 0.5),
                Q3 = calc.Percentile(nonEliteData.Select(d => (double)d.Kal!).ToList(), 0.75),
                Min = nonEliteData.Select(d => (double)d.Kal!).Min(),
                Max = nonEliteData.Select(d => (double)d.Kal!).Max()
            });

            return result;
        }
        public async Task<IEnumerable<PlotItem>> GetZCurrentPlotData()
        {
            var allData = await (from c in _dbContext.Coefficients
                                 join h in _dbContext.Horses
                                 on c.HorseOId equals h.OId
                                 where c.ZCurrentBPR != null
                                 select new { 
                                    h.Age,
                                    c.ZCurrentBPR
                                 }).ToListAsync();

            Calculator calc = new Calculator();


            var groupResult = allData.GroupBy(x => x.Age)
                .Select(x => new PlotItem { 
                    Category = "ZCurrentBPR",
                    Title = $"{x.Key}",
                    Max = (double)x.Max(v => v.ZCurrentBPR),
                    Min = (double)x.Min(v => v.ZCurrentBPR),
                    Info = x.Count(),
                    Q1 = calc.Percentile(x.Select(v => (double)v.ZCurrentBPR!).ToList(), 0.25),
                    Median = calc.Percentile(x.Select(v => (double)v.ZCurrentBPR!).ToList(), 0.5),
                    Q3 = calc.Percentile(x.Select(v => (double)v.ZCurrentBPR!).ToList(), 0.75),
                })
                .OrderBy(x => x.Title);


            return groupResult;
        }
        public async Task<IEnumerable<PlotItem>> GetZHistoricalPlotData()
        {
            var allData = await (from c in _dbContext.Coefficients
                                 join h in _dbContext.Horses
                                 on c.HorseOId equals h.OId
                                 where c.ZHistoricalBPR != null
                                 select new
                                 {
                                     h.Age,
                                     c.ZHistoricalBPR
                                 }).ToListAsync();

            Calculator calc = new Calculator();


            var groupResult = allData.GroupBy(x => x.Age)
                .Select(x => new PlotItem
                {
                    Category = "ZCurrentBPR",
                    Title = $"{x.Key}",
                    Max = (double)x.Max(v => v.ZHistoricalBPR),
                    Min = (double)x.Min(v => v.ZHistoricalBPR),
                    Info = x.Count(),
                    Q1 = calc.Percentile(x.Select(v => (double)v.ZHistoricalBPR!).ToList(), 0.25),
                    Median = calc.Percentile(x.Select(v => (double)v.ZHistoricalBPR!).ToList(), 0.5),
                    Q3 = calc.Percentile(x.Select(v => (double)v.ZHistoricalBPR!).ToList(), 0.75),
                })
                .OrderBy(x => x.Title);


            return groupResult;
        }

        public Horse[] SearchHorsesEx(string q, string sex)
        {
            Horse[] result;

            var query = Queries.Search_Horses_Ex(q, sex);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = conn.Query<Horse>(query).ToArray();
            }

            return result;
        }

        public async Task SetMtDNAFlags(string startHorseOId, string endHorseOId, bool flag)
        {
            var query = string.Format(Queries.Set_MtDNA_Flags, startHorseOId, endHorseOId, flag ? 1 : 0);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                await conn.ExecuteAsync(query);
            }
        }

        public async Task<int> SaveBPRs(string horseOId, double currentBPR, double? zCurrentBPR, double? historicalBPR, double? zHistoricalBPR)
        {
            string zCurrentBPRStr, historicalBPRStr, zHistoricalBPRStr;

            if (zCurrentBPR == null) zCurrentBPRStr = "NULL";
            else zCurrentBPRStr = zCurrentBPR.ToString();

            if (historicalBPR == null) historicalBPRStr = "NULL";
            else historicalBPRStr = historicalBPR.ToString();

            if (zHistoricalBPR == null) zHistoricalBPRStr = "NULL";
            else zHistoricalBPRStr = zHistoricalBPR.ToString();

            int result;
            var query = string.Format(Queries.Save_BPRs, horseOId, currentBPR, zCurrentBPRStr, historicalBPRStr, zHistoricalBPRStr);
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.ExecuteAsync(query);
            }

            return result;
        }

        public async Task<Horse> GetHorseByName(string name)
        {
            Horse result = null;
            using (var conn = new SqlConnection(_connectionString.Value))
            {
                result = await conn.QueryFirstOrDefaultAsync<Horse>(Queries.Get_Horse_By_Name, new { Name = name });
            }
            return result;
        }

        #region Private Methods

        private async Task<IEnumerable<Horse>> GetParents(string oId)
        {
            return await (from h in _dbContext.Horses
                          join r in _dbContext.Relationships
                          on h.OId equals r.ParentOId
                          where r.HorseOId == oId
                          select h).ToListAsync();
        }

        private async Task<IEnumerable<Horse>> GetChildren(string oId)
        {
            return await (from h in _dbContext.Horses
                          join r in _dbContext.Relationships
                          on h.OId equals r.HorseOId
                          where r.ParentOId == oId
                          orderby h.Age, h.Name
                          select h).ToListAsync();
        }
        #endregion
    }
}