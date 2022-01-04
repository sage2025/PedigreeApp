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
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _repo;
        private readonly IHorseService _horseService;
        private readonly IMapper _mapper;

        public PositionService(
            IPositionRepository repo,
            IHorseService horseService,
            IMapper mapper)
        {
            _repo = repo;
            _horseService = horseService;
            _mapper = mapper;
        }

        public async Task<int> CreatePositionAsync(PositionDTO position)
        {
            var result = await _repo.Create(_mapper.Map<PositionDTO, Position>(position));

            if (position.Place == 1)
            {
                await _horseService.DoUpdatePedigreeForHorse(position.HorseOId);
            }
            return result;
        }

        public async Task UpdatPositionAsync(PositionDTO position)
        {
            var existingPosition = await _repo.GetById(position.Id);
            existingPosition.Place = position.Place;
            existingPosition.HorseOId = position.HorseOId;

            existingPosition.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(position.Id, existingPosition);

            if (position.Place == 1)
            {
                await _horseService.DoUpdatePedigreeForHorse(position.HorseOId);
            }

        }


        public async Task<DataListDTO<PositionDTO>> SearchPositionsByPageAndTotalAsync(int raceId, string q, int page, int size)
        {
            DataListDTO<PositionDTO> positionsDto = null;
            var finaList = new List<PositionDTO>();
            try
            {
                var positionsWithTotal = await _repo.SearchPaginatedWithTotalAsync(raceId, q, page, size);

                var dtos = positionsWithTotal.Data.Select(h => _mapper.Map<Position, PositionDTO>(h)).ToList();

                if (dtos.Count > 0)
                {
                    finaList.AddRange(dtos);
                }
                positionsDto = new DataListDTO<PositionDTO>
                {
                    Data = finaList,
                    Total = positionsWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            return positionsDto;
        }

        public async Task DeletePositionAsync(int positionId)
        {
            await _repo.Delete(positionId);
        }

        public async Task<bool> HasRaceResult(string horseOId)
        {
            return await _repo.HasRaceResult(horseOId);
        }
    }

}
