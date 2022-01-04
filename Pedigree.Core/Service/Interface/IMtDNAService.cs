using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IMtDNAService
    {
        Task<int> CreateHaploTypeAsync(HaploTypeDTO haploType);
        Task UpdatHaploTypeAsync(HaploTypeDTO haploType);
        Task<HaploGroupDTO[]> GetHaploGroups();
        Task<HaploGroupDTO[]> GetHorseHaploGroups(int horseId);
        Task<HaploGroupDTO[]> GetHypotheticalHaploGroups(int maleId, int femaleId);
        Task DeleteHaploTypeAsync(int typeId);
        Task UpdateHaploGroupAsync(int groupId, string color);
        Task<HaploGroupStallionDTO[]> GetHaploGroupsStallion(int maleId);
        Task<HaploGroupDistanceDTO[]> GetHaploGroupsDistance();
        Task<int> CreateMtDNAFlag(MtDNAFlagDTO flagDTO);
        Task<MtDNAFlagDTO[]> GetMtDNAFlags();
        Task DeleteMtDNAFlag(int flagId);
        Task<HaploGroupDTO[]> GetSimpleHaploGroups();
    }
}
