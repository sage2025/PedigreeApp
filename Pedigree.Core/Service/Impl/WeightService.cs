using AutoMapper;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Interface;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Impl
{
    public class WeightService : IWeightService
    {
        private readonly IWeightRepository _repo;
        private readonly IMapper _mapper;

        public WeightService(IWeightRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateWeightAsync(WeightDTO weight)
        {
            return await _repo.Create(_mapper.Map<WeightDTO, Weight>(weight));
        }

        public async Task<WeightDTO> GetWeightAsync(int id)
        {
            var weight = await _repo.GetById(id);
            return _mapper.Map<Weight, WeightDTO>(weight);
        }

        public async Task UpdatWeightAsync(WeightDTO weight)
        {
            var existingWeight = await _repo.GetById(weight.Id);
            existingWeight.Country = weight.Country;
            existingWeight.Distance = weight.Distance;
            existingWeight.Surface = weight.Surface;
            existingWeight.Type = weight.Type;
            existingWeight.Status = weight.Status;
            existingWeight.Value = weight.Value;

            existingWeight.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(weight.Id, existingWeight);
        }

        public async Task<DataListDTO<WeightDTO>> SearchWeightsByPageAndTotalAsync(string q, int page, string sort, string direction, int size)
        {
            DataListDTO<WeightDTO> weightsDto = null;
            var finaList = new List<WeightDTO>();
            try
            {
                var weightsWithTotal = await _repo.SearchPaginatedWithTotalAsync(q, page, sort, direction, size);

                var dtos = weightsWithTotal.Data.Select(h => _mapper.Map<Weight, WeightDTO>(h)).ToList();

                if (dtos.Count > 0)
                {
                    finaList.AddRange(dtos);                    
                }
                weightsDto = new DataListDTO<WeightDTO>
                {
                    Data = finaList,
                    Total = weightsWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            return weightsDto;
        }

        public async Task DeleteWeightAsync(int weightId)
        {
            await _repo.Delete(weightId);
        }
    }

}
