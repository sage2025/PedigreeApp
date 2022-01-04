using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IRelationshipRepository : IGenericRepository<Relationship>
    {
        Task<bool> DeleteRelationshipByOIdComoboAsync(string horseOid, string parentOid);
        Task<Relationship> GetRelationshipByOIdComoboAsync(string horseOid, string parentType);

        Task<IEnumerable<Relationship>> GetImmediateParents(string horseOid);

        Task<IEnumerable<Horse>> GetChildrenHorses(string parentOId);

        Task<HorsesWithTotal> GetHorseOffspringsAsync(int id);
        Task<HorsesWithTotal> GetHorseSibilingsAsync(int id, string parentType = "Mother");
        Task<HorsesWithTotal> GetHorseFemaleTailAsync(int id);
        Task<HorsesWithTotal> GetUniqueAncestorsAsync(int horseId, int maxGen = 10);
        Task<IEnumerable<Inbreed>> GetInbreedingsAsync(string horseOId, string[] oids);
        Task<bool> DeleteRelationshipByParentTypeAsync(string horseOId, string parentType);
        Task<HorsesWithTotal> GetGrandparents(string sort, string order, int page, int size);
        Task<HorsesWithTotal> GetSWOffsprings(int maleHorseId, int femaleHorseId);
        Task<IEnumerable<Horse>> GetSWOffspringsBySire(int maleHorseId);
        Task<IEnumerable<Horse>> GetSWOffspringByBroodmareSire(int maleHorseId);
        Task<IEnumerable<Horse>> GetStakeWinnersBySireDescendants(int maleHorseId, int gen = 5);
        Task<IEnumerable<Horse>> GetStakeWinnersByBroodmareSireDescendants(int maleHorseId, int gen = 5);
        Task<Horse> GetParent(int horseId, string parentType);
        Task<IEnumerable<Horse>> GetStakeWinnersByWildcard1Search(int horse1Id, int horse2Id);
        Task<IEnumerable<Horse>> GetStakeWinnersByWildcard2Search(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id);
        Task<IEnumerable<Horse>> GetStakeWinnersByWildcardQueryByPosition(Dictionary<int, int> searches);
        Task<IEnumerable<HorseHeirarchy>> GetStakeWinnersByMareDescendants(int femaleId, int gen = 5);
        Task<IEnumerable<HorseHeirarchy>> GetHorsesForFemaleLineSearch(int femaleId);
        Task<IEnumerable<HorseMtDNALookupDTO>> GetHorsesForMtDNALookup(int haploGroupId);
    }
}
