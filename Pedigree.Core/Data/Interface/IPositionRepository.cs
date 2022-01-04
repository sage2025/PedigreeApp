using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IPositionRepository : IGenericRepository<Position>
    {
        Task<EntitiesWithTotal<Position>> SearchPaginatedWithTotalAsync(int raceId, string q, int page, int size);
        Task<Position> GetPosition(string horseOId, int raceId);
        Task<bool> HasRaceResult(string horseOId);
        Task<IEnumerable<Position>> GetRacePositions(int raceId);
        Task<IEnumerable<Position>> GetAllPositions();
    }
}
