using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IWeightRepository : IGenericRepository<Weight>
    {
        Task<EntitiesWithTotal<Weight>> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size);
    }
}
