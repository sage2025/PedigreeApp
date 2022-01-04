using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IHorseRepository : IGenericRepository<Horse>
    {
        HorsesWithTotal GetPaginatedWithTotal(int page, string sort, string direction, int size);

        HorsesWithTotal SearchNameStartsWithPaginatedWithTotal(string q, int page, int size);
        HorsesWithTotal SearchPaginatedWithTotal(string q, int page, string sort, string direction, int size);
        Task<HorsesWithTotal> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size);
        Task<bool> DuplicateCheck(string name, int age, string sex, string country);
        Task<HorsesWithTotal> SearchWithGenderPaginatedWithTotalAsync(string q, string gender, int page, string sort, string direction, int size);

        Task<HorseWithParentChildren> GetHorseWithParentChildrenById(int id);
        Task<Horse> GetHorseById(int id);

        Task<HorseWithParentChildren> GetHorseWithParentChildrenByOId(string Oid);

        Task<IEnumerable<Horse>> GetParentsOrChildren(string Oid, bool needParents);

        Task<bool> HasChildren(string oid);

        Task<IEnumerable<int>> GetHorseIdsForCoefficient();
        Task<IEnumerable<int>> GetHorseIdsForProbOrig(int limit = 300);
        Task<int> GetHorsesCountForProbOrig();

        Task<IEnumerable<int>> GetHorseIdsForGRain(int limit = 300);
        Task<IEnumerable<int>> GetHorseIdsForUniqueAncestorsCount(int limit = 300);
        Task<IEnumerable<int>> GetHorseIdsForPedigree(int limit = 300);

        Task<IEnumerable<Horse>> GetModernHorses();
        Task<IEnumerable<HorseHeirarchy>> GetHorseHeirarchyBottomUp(int id, int generationLevel, string SD);
        Task<IEnumerable<Horse>> GetHorsesByHaploTypes(int[] types);

        Task<bool> SoftDelete(int id);
        Task<Horse> GetByOid(string Oid);
        Task<HorsePedigree> GetPedigree(int horseId, int gen = 5, int yob = 0);
        Task<Horse> GetHorseByName(string name);
        Task<HorsePedigree> GetPedigreeComp(int horseId, int gen = 5, int yob = 0);
        Task<HorsePedigree> GetHypotheticalPedigree(int sireId, int damId, int gen = 5, int yob = 0);
        Task<HorsePedigree> GetFullPedigree(int horseId, int gen = 5, int yob = 0);
        Task<HorsePedigree> GetHypotheticalFullPedigree(int sireId, int damId, int gen = 5, int yob = 0);
        Task<HorsesWithTotal> GetIncompletedPedigreeHorses(int year, string sort, string order, int page, int size);
        Task<IEnumerable<HorseTwin>> GetTwinDams();
        Task<HorsesWithTotal> GetFounders();

        Task<HorsesWithTotal> GetInbreedingHorses(string oid, string sort, string order, int page, int size);

        Task<IEnumerable<Horse>> AttatchParents(IEnumerable<Horse> horses);

        Task<Coefficient> GetCoefficient(string oid);
        Task<Coefficient> GetCoefficientByHorseId(int horseId);
        Task<int> CreateCoefficient(Coefficient coefficient);
        Task UpdateCoefficient(Coefficient coefficient);
        Task UpdateFamilyForTailFemale(int horseId, string family);
        Task UpdateMtDNAForTailFemale(int horseId, int mtDNA);
        Task<IEnumerable<Horse>> GetCheckedFounders();
        Task CalculateCOIDs();
        Task CalculateCOID(string oId);
        Task<Pedig> GetPedigByHorseId(int horseId);
        Task<Pedig> GetPedig(string oid);
        Task<bool> ExistPedig(string oid);
        Task<int> CreatePedig(Pedig pedig);
        Task UpdatePedig(Pedig pedig);
        Task<IEnumerable<Pedig>> GetPedigsForProbOrigs();
        Task<IEnumerable<Horse>> GetEliteHorsesInHaploGroup(HaploGroupDTO group);
        Task<IEnumerable<Horse>> GetNonEliteHorsesInHaploGroup(HaploGroupDTO group);
        Task<Ancestry> GetAncestry(string ancestorOId);
        Task UpdateAncestry(Ancestry ancestry);
        Task<int> CreateAncestry(Ancestry ancestry);
        Task DeleteAncestry(Ancestry ancestry);
        Task<IEnumerable<Ancestry>> GetAncestries(bool byQuery=true);
        Task<IEnumerable<PlotItem>> GetPopulationData();
        Task<IEnumerable<Horse>> GetUniqueAncestors(int horseId, int gen = 10);
        Task<int> GetUniqueAncestorsCount(int horseId, int gen = 10);
        Task<IEnumerable<PlotItem>> GetZCurrentPlotData();
        Task<IEnumerable<PlotItem>> GetZHistoricalPlotData();
        Task UpdatePedigree(int horseId);
        Task UpdateProbOrigs(int horseId, List<ProbOrig> probOrigs);
        Task DeleteProbOrigs();
        Task DeleteGRainData();
        Horse[] SearchHorsesEx(string q, string sex);
        Task SetMtDNAFlags(string startHorseOId, string endHorseOId, bool flag);
        Task<int> SaveBPRs(string horseOId, double currentBPR, double? zCurrentBPR, double? historicalBPR, double? zHistoricalBPR);
    }
}
