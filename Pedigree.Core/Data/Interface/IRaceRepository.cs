using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Pedigree.Core.Data.Interface
{
    public interface IRaceRepository : IGenericRepository<Race>
    {
        Task<EntitiesWithTotal<Race>> SearchPaginatedWithTotalAsync(string q, int page, string sort, string direction, int size);
        Task<bool> HasResult(int raceId);
        Task<Race> GetRace(string name, DateTime date, string country);
        Task<IEnumerable<HorseRace>> GetRacesForHorse(string oId, string sort, string order);
        Task<IEnumerable<HorseRace>> GetRacesForHorses(string[] oIds);
        Task<IEnumerable<HorseRace>> GetRacesForStakeWinners();
        Task<IEnumerable<Race>> GetAllRaces();
        Task<IEnumerable<Race>> GetHistRaces();
        double GetStallionAvgRunnersCount(string oId);
        int GetStallionRunnersCount(string oId);
        int GetStallionWinnersCount(string oId);
        Task<double> GetExpectedStallionWnrsValue();
    }
}
