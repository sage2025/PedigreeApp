using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IRaceService
    {
        Task<int> CreateRaceAsync(RaceDTO race);
        Task<RaceDTO> GetRaceAsync(int id);
        Task UpdatRaceAsync(RaceDTO race);
        Task<DataListDTO<RaceDTO>> SearchRacesByPageAndTotalAsync(string q, int page, string sort, string order, int size);
        Task<bool> HasResult(int raceId);
        Task DeleteRaceAsync(int raceId);
    }
}
