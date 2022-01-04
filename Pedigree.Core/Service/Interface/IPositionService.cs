using Pedigree.Core.Data.DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IPositionService
    {
        Task<int> CreatePositionAsync(PositionDTO position);
        Task<DataListDTO<PositionDTO>> SearchPositionsByPageAndTotalAsync(int raceId, string q, int page, int size);
        Task DeletePositionAsync(int positionId);
        Task<bool> HasRaceResult(string horseOId);
        Task UpdatPositionAsync(PositionDTO position);
    }
}
