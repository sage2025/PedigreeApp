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
    public class RaceService : IRaceService
    {
        private readonly IRaceRepository _repo;
        private readonly IMapper _mapper;

        public RaceService(IRaceRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<int> CreateRaceAsync(RaceDTO race)
        {
            return await _repo.Create(_mapper.Map<RaceDTO, Race>(race));
        }

        public async Task<RaceDTO> GetRaceAsync(int id)
        {
            var race = await _repo.GetById(id);
            return _mapper.Map<Race, RaceDTO>(race);
        }

        public async Task UpdatRaceAsync(RaceDTO race)
        {
            var existingRace = await _repo.GetById(race.Id);
            existingRace.Name = race.Name;
            existingRace.Date = race.Date;
            existingRace.Country = race.Country;
            existingRace.Distance = race.Distance;
            existingRace.Surface = race.Surface;
            existingRace.Type = race.Type;
            existingRace.Status = race.Status;

            existingRace.UpdatedAt = DateTime.UtcNow;

            await _repo.Update(race.Id, existingRace);
        }

        public async Task<DataListDTO<RaceDTO>> SearchRacesByPageAndTotalAsync(string q, int page, string sort, string direction, int size)
        {
            DataListDTO<RaceDTO> racesDto = null;
            var finaList = new List<RaceDTO>();
            try
            {
                var racesWithTotal = await _repo.SearchPaginatedWithTotalAsync(q, page, sort, direction, size);

                var dtos = racesWithTotal.Data.Select(h => _mapper.Map<Race, RaceDTO>(h)).ToList();

                if (dtos.Count > 0)
                {
                    var ExactMatchs = dtos.Where(f => f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                    if (ExactMatchs != null && ExactMatchs.Count() > 0)
                    {
                        // insert placeholder for exact

                        finaList.AddRange(ExactMatchs);
                        var remaining = dtos.Where(f => !f.Name.Equals(q, StringComparison.OrdinalIgnoreCase));
                        if (remaining != null && remaining.Count() > 0)
                        {
                            finaList.AddRange(remaining);
                        }
                    }
                    else
                    {
                        finaList.AddRange(dtos);
                    }
                }
                racesDto = new DataListDTO<RaceDTO>
                {
                    Data = finaList,
                    Total = racesWithTotal.Total
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw;
            }

            return racesDto;
        }

        public async Task<bool> HasResult(int raceId)
        {
            return await _repo.HasResult(raceId);
        }

        public async Task DeleteRaceAsync(int raceId)
        {
            await _repo.Delete(raceId);
        }
    }

}
