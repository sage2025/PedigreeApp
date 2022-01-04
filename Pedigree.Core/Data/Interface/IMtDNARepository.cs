using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IMtDNARepository : IGenericRepository<HaploGroup>
    {
        Task<HaploGroup[]> GetHaploGroups();
        Task<int> CreateHaploType(HaploType haploType);
        Task DeleteHaploType(int typeId);
        Task UpdateHaploType(HaploType haploType);
        Task<int> CreateMtDNAFlag(MtDNAFlag mtDNAFlag);
        Task<MtDNAFlag[]> GetMtDNAFlags();
        Task DeleteMtDNAFlag(int flagId);
        Task<MtDNAFlag> GetMtDNAFlag(int flagId);
    }
}
