

using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IRelationshipService
    {
        Task CreateRelationshipAsync(RelationshipDTO relationship);

        Task UpdatRelationshipAsync(RelationshipDTO relationship);

        Task DeleteRelationshipAsync(int id);

        Task<bool> DeleteByOidComoboAsync(string horseOid, string parentOId);
        Task<RelationshipDTO> GetByOidComoboAsync(string horseOid, string parentType);


        Task<HorseListDTO> GetHorseOffspringsAsync(int id);
        Task<HorseListDTO> GetHorseSibilingsAsync(int id);
        Task<HorseListDTO> GetHorseFemaleTailAsync(int id);
        Task<HorseListDTO> GetUniqueAncestorsAsync(int horseId);
        Task<HorseListDTO> GetUniqueAncestorsAsync(int maleHorseId, int femaleHorseId, int gen = 9);
        Task<HorseInbreedingListDTO> GetInbreedingsAsync(string horseOId, string sort, string order, int page, int size);
        Task<bool> DeleteByParentTypeAsync(string horseOId, string parentType);
        Task<HorseListDTO> GetGrandparents(string sort, string order, int page, int size);
    }
}
