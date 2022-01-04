using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pedigree.Core.Service.Interface
{
    public interface IHorseService
    {
        Task<IEnumerable<HorseDTO>> GetHorsesByPageAsync(int page, int size);
        Task<HorseDTO> GetHorseAsync(int id);

        HorseListDTO GetHorsesByPageAndTotal(int page, string sort, string direction, int size);

        HorseListDTO SearchHorsesByPageAndTotal(string q, int page, int size);

        HorseListDTO SearchHorsesByPageAndTotal(string q, int page, string sort, string direction, int size);
        Task<HorseListDTO> SearchHorsesByPageAndTotalAsync(string q, int page, string sort, string direction, int size);
        Task<GrainItem> DoCalculateGRainForHorse(int horseId);
        Task<GrainItem> DoCalculateGRainByPedigree(HorsePedigree pedigree);
        Task<HorseListDTO> SearchHorsesByGenderPageAndTotalAsync(string q, string gender, int page, string sort, string direction, int size);

        Task<HorseWithParentChildrenDTO> GetHorseWithParentChildrenById(int id);
        Task<HorseDTO> GetHorseById(int id);

        Task<HorseWithParentChildrenDTO> GetHorseWithParentChildrenByOId(string Oid);

        Task<IEnumerable<HorseDTO>> GetParentsOrChildrenAsync(string Oid, bool needParents);
        Task CalculateBPRs();
        Task<bool> HasChildren(string oid);
        Task SetMtDNAForFounders();
        Task<HorseHeirarchyDataDTO> GetHeriarchy(int id, int generationLevel = 5);
        Task DoCalculateUniqueAncestorsCount();
        Task<HorseHeirarchyDataDTO> GetHypotheticalHeriarchy(int maleHorseId, int femaleHorseId, int generationLevel = 5);
        Task<IEnumerable<LinebreedingItem>> GetLinebreedings(int id, int genLevel = 5);
        Task<IEnumerable<LinebreedingItem>> GetLinebreedingsForHypothetical(int maleHorseId, int femaleHorseId, int genLevel = 5);
        Task<IEnumerable<Par3Item>> GetEquivalents(int horseId);
        Task PickupParentData(string parentOId);
        Task<Horse> GetHorseByOId(string oid);
        Task<IEnumerable<Par3Item>> GetEquivalentsForHypothetical(int maleHorseId, int femaleHorseId);
        Task<HorseListDTO> GetCommonAncestors(int horseId1, int horseId2);
        Task<HorseListDTO> GetIncompletedPedigreeHorses(int year, string sort, string order, int page, int size);
        Task<IEnumerable<HorseTwinDTO>> GetTwinDams();
        Task<HorseListDTO> GetFounders();
        Task<AncestriesDataDTO> GetAncestriesData();
        Task<IEnumerable<PlotItem>> GetPopulationData();
        Task<IEnumerable<PlotItem>> GetZCurrentPlotData();
        Task<IEnumerable<PlotItem>> GetZHistoricalPlotData();

        Task DoCalculateCOIsForAllHorses();
        Task<Coefficient> DoCalculateCOIs(HorsePedigree pedigree, bool forHypo = false);

        Task DoCalculateProbOrig();
        Task DoCalculateAncestryLog();
        Task DoCalculateGRain();
        Task DoUpdatePedigree();
        Task DoUpdatePedigreeForHorse(int horseId);
        Task DoUpdatePedigreeForHorse(string horseOId);

        Task SetFounderAsync(int horseId, bool isFounder);
        Task SetMtDNAAsync(int horseId, int mtDNA);
        Task<IEnumerable<HorseRaceDTO>> GetRaces(int horseId, string sort, string order);
        Task<IEnumerable<HorseDTO>> GetHorsesForSireSearch(int maleHorseId, int femaleHorseId);
        Task<IEnumerable<HorseDTO>> GetHorsesForSireBroodmareSireSearch(string type, int maleHorseId);
        Task<IEnumerable<HorseDTO>> GetHorsesForSirelineSearch(string type, int maleId);
        Task<SireCrossData> GetSiresCrossData(int maleId1, int maleId2);
        Task<IEnumerable<HorseDTO>> GetHorsesForWildcard1Search(int horse1Id, int horse2Id);
        Task<IEnumerable<HorseDTO>> GetHorsesForWildcard2Search(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id);
        Task<IEnumerable<HorseDTO>> GetHorsesForWildcardQueryByPosition(Dictionary<int, int> searches);
        Task<IEnumerable<HorseHeirarchy>> GetHorsesForFamilyStakeSearch(int femaleId, int gen);
        Task<HorseHeirarchyDataDTO> GetHorseHierarchyDataForFemaleLineSearch(int femaleId);
        Task<IEnumerable<HorseMtDNALookupDTO>> GetHorsesForMtDNALookup(int haploGroupId);
        Horse[] SearchHorsesEx(string q, string sex);
        Task CalculateCOIDs();
        #region Add/UPdate/Delete
        Task<HorseDTO> CreateHorseAsync(HorseDTO horse);

        Task UpdateHorseAsync(HorseDTO horse);

        Task DeleteHorseAsync(int id);
        Task RemoveParentData(string horseOId);
        // Task AddAuction(string date, string name);
        #endregion
    }
}
