using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IWeightService
    {
        Task<int> CreateWeightAsync(WeightDTO weight);
        Task<WeightDTO> GetWeightAsync(int id);
        Task UpdatWeightAsync(WeightDTO weight);
        Task<DataListDTO<WeightDTO>> SearchWeightsByPageAndTotalAsync(string q, int page, string sort, string order, int size);
        Task DeleteWeightAsync(int weightId);
    }
}
