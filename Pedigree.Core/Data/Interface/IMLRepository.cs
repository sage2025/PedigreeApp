using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IMLRepository : IGenericRepository<MLModel>
    {
        Task<int> CreateMLModel(MLModel mlModel);
        Task<IEnumerable<MLModelData>> GetMLModelData();
        Task<MLModel> GetLastMLModel();
    }
}
