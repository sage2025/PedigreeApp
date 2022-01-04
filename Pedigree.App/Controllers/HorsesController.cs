using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Data.Entity;
using Pedigree.Core.Data.Entity.InMemoryOnly;
using Pedigree.Core.Service.Impl;
using Pedigree.Core.Service.Interface;

namespace Pedigree.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HorsesController : ControllerBase
    {
        private IHorseService _horseService;
        private IValidationService _validationService;
        private IPositionService _positionService;
        public HorsesController(IHorseService horseService, IValidationService validationService, IPositionService positionService)
        {
            _horseService = horseService;
            _validationService = validationService;
            _positionService = positionService;
        }


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Horses(string q, string sort, string order, int page, int size = 100)
        {
            HorseListDTO result = null;
            try
            {
                if (!string.IsNullOrEmpty(q))
                {
                    result = await _horseService.SearchHorsesByPageAndTotalAsync(q, page, sort, order, size);
                }

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("hypothetical/search")]
        public async Task<IActionResult> HorsesHypotheicalMating(string maleq, string femaleq, string sort, string order, int page, int size = 30)
        {
            HorseListDTO result = null;
            try
            {
                if (!string.IsNullOrEmpty(maleq) && !string.IsNullOrEmpty(femaleq))
                {
                    var maleResult = await _horseService.SearchHorsesByGenderPageAndTotalAsync(maleq, "Male", page, sort, order, size);
                    if (maleResult.Horses != null && maleResult.Horses.Count() > 0)
                    {
                        var temp = maleResult.Horses.ToList();
                        //var maleGroupHeaderFiller = new HorseDTO() { Name = "", ShowHeader = true, ShowHeaderText = "Sire", Sex = "Male" };
                        //temp.Insert(0, maleGroupHeaderFiller);
                        maleResult.Horses = temp;
                    }
                    var femaleResult = await _horseService.SearchHorsesByGenderPageAndTotalAsync(femaleq, "Female", page, sort, order, size);
                    if (femaleResult.Horses != null && femaleResult.Horses.Count() > 0)
                    {
                        var temp = femaleResult.Horses.ToList();
                        //var femaleGroupHeaderFiller = new HorseDTO() { Name = "", ShowHeader = true, ShowHeaderText = "Dam", Sex = "Female" };
                        //temp.Insert(0, femaleGroupHeaderFiller);
                        femaleResult.Horses = temp;
                    }
                    // Assign to single result
                    var combinedList = new List<HorseDTO>();
                    combinedList.AddRange(maleResult.Horses);
                    combinedList.AddRange(femaleResult.Horses);
                    result = new HorseListDTO();
                    result.Horses = combinedList;
                    result.Total = maleResult.Total + femaleResult.Total;
                }

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("search/{q}")]
        public IActionResult SearchHorses(string q)
        {

            try
            {
                var result = _horseService.SearchHorsesByPageAndTotal(q, 1, "age", "desc", 60);
                return Ok(result.Horses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search_ex/{q}")]
        public IActionResult SearchHorsesEx(string q, string sex)
        {
            try
            {
                var result = _horseService.SearchHorsesEx(q, sex);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("heirarchy/{id}")]
        public async Task<IActionResult> GetHeirarchyById(int id)
        {
            try
            {
                var result = await _horseService.GetHeriarchy(id, 5);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("heirarchy/hypothetical")]
        public async Task<IActionResult> GetHypotheticalHeirarchy([FromQuery] int maleHorseId, [FromQuery] int femaleHorseId)
        {
            try
            {
                var result = await _horseService.GetHypotheticalHeriarchy(maleHorseId, femaleHorseId, 5);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddHorse([FromBody] HorseDTO horse)
        {
            try
            {
                var noDuplicate = await _validationService.IsDuplicateHorseAsync(horse);
                if (!noDuplicate.Output)
                {
                    var result = await _horseService.CreateHorseAsync(horse);
                    return Ok(result);
                }
                else
                {
                    return BadRequest(noDuplicate.Message);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{horseId}")]
        public async Task<IActionResult> UpdateHorse([FromRoute] int horseId, [FromBody] HorseDTO horse)
        {
            try
            {
                horse.Id = horseId;
                await _horseService.UpdateHorseAsync(horse);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> HorseByIdWithParentAndChildren(int id)
        {
            HorseWithParentChildrenDTO result = null;
            try
            {
                result = await _horseService.GetHorseWithParentChildrenById(id);
                if (result == null)
                    return BadRequest(new { message = "Horse data now found." });
            }
            catch (Exception ex)
            {
                throw; // TODO:
            }
            return Ok(result);
        }

        [HttpGet("{id}/single")]
        public async Task<IActionResult> HorseById(int id)
        {
            HorseDTO result = null;
            try
            {
                result = await _horseService.GetHorseById(id);
                if (result == null)
                    return BadRequest(new { message = "Horse data now found." });
            }
            catch (Exception ex)
            {
                throw; // TODO:
            }
            return Ok(result);
        }

        [HttpGet("o/{oId}")]
        public async Task<IActionResult> HorseByIdWithParentAndChildren(string oId)
        {
            HorseWithParentChildrenDTO result = null;
            try
            {
                result = await _horseService.GetHorseWithParentChildrenByOId(oId);
                if (result == null)
                    return BadRequest(new { message = "Horse data now found." });
            }
            catch (Exception ex)
            {
                throw; // TODO:
            }
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Post([FromBody] HorseDTO horse)
        {
            try
            {
                await _horseService.UpdateHorseAsync(horse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok();
        }

        [HttpGet("linkage/{oid}")]
        public async Task<IActionResult> HasLinkage(string oid)
        {
            var result = new HorseLinkage();
            try
            {
                result.HasChildren = await _horseService.HasChildren(oid);
                result.HasRaceResult = await _positionService.HasRaceResult(oid);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _horseService.DeleteHorseAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("branch/{oId}/{needParents}")]
        public async Task<IActionResult> HorseParentsOrChildren(string oId, int needParents)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                switch (needParents)
                {
                    case 0: // children
                    case 1: // parents
                        result = await _horseService.GetParentsOrChildrenAsync(oId, Convert.ToBoolean(needParents));
                        break;
                    default:
                        return BadRequest("Not a valid switch");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }


            if (result == null)
                return BadRequest("No Data was found");

            return Ok(result);
        }

        [HttpGet("linebreedings/{horseId}")]
        public async Task<IActionResult> Linebreedings(int horseId)
        {
            try
            {
                return Ok(await _horseService.GetLinebreedings(horseId, 10));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("linebreedings_hypothetical")]
        public async Task<IActionResult> LinebreedingsForHypothetical(int maleHorseId, int femaleHorseId)
        {
            try
            {
                return Ok(await _horseService.GetLinebreedingsForHypothetical(maleHorseId, femaleHorseId, 9));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equivalents/{horseId}")]
        public async Task<IActionResult> Equivalents(int horseId)
        {
            try
            {
                return Ok(await _horseService.GetEquivalents(horseId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("equivalents_hypothetical")]
        public async Task<IActionResult> EquivalentsForHypothetical(int maleHorseId, int femaleHorseId)
        {
            try
            {
                return Ok(await _horseService.GetEquivalentsForHypothetical(maleHorseId, femaleHorseId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("common_ancestors")]
        public async Task<IActionResult> GetCommonAncestors(int horseId1, int horseId2)
        {
            try
            {
                return Ok(await _horseService.GetCommonAncestors(horseId1, horseId2));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("incompleted_pedigree_horses")]
        public async Task<IActionResult> GetIncompletedPedigreeHorses(int year, string sort, string order, int page, int size=100)
        {
            try
            {
                return Ok(await _horseService.GetIncompletedPedigreeHorses(year, sort, order, page, size));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("twins")]
        public async Task<IActionResult> GetTwinDams()
        {
            try
            {
                return Ok(await _horseService.GetTwinDams());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("founders")]
        public async Task<IActionResult> GetFounders()
        {
            try
            {
                return Ok(await _horseService.GetFounders());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{horseId}/set_founder")]
        public async Task<IActionResult> SetFounder([FromRoute] int horseId, [FromBody] HorseFounderDTO data)
        {
            try
            {
                await _horseService.SetFounderAsync(horseId, data.IsFounder);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{horseId}/set_mtdna")]
        public async Task<IActionResult> SetMtDNA([FromRoute] int horseId, [FromBody] HorseMtDNADTO data)
        {
            try
            {
                await _horseService.SetMtDNAAsync(horseId, data.MtDNA);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("races/{horseId}")]
        public async Task<IActionResult> Races(int horseId, string sort, string order)
        {
            try
            {
                return Ok(await _horseService.GetRaces(horseId, sort, order));
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("ancestor_data")]
        public async Task<IActionResult> GetAncestorData()
        {
            try
            {
                return Ok(await _horseService.GetAncestriesData());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("population_data")]
        public async Task<IActionResult> GetPopulationData()
        {
            try
            {
                return Ok(await _horseService.GetPopulationData());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("zcurrent_plot_data")]
        public async Task<IActionResult> GetZCurrentPlotData()
        {
            try
            {
                return Ok(await _horseService.GetZCurrentPlotData());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("zhistorical_plot_data")]
        public async Task<IActionResult> GetZHistoricalPlotData()
        {
            try
            {
                return Ok(await _horseService.GetZHistoricalPlotData());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("sire/search")]
        public async Task<IActionResult> HorsesSireSearch(int maleId, int femaleId)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                result = await _horseService.GetHorsesForSireSearch(maleId, femaleId);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("sire_broodmare_sire/search")]
        public async Task<IActionResult> HorsesSireBroodmareSireSearch(string type, int maleId)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                result = await _horseService.GetHorsesForSireBroodmareSireSearch(type, maleId);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("sire_line/search")]
        public async Task<IActionResult> HorsesSirelineSearch(string type, int maleId)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                result = await _horseService.GetHorsesForSirelineSearch(type, maleId);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("sires_cross_data")]
        public async Task<IActionResult> SiresCrossData(int maleId1, int maleId2)
        {
            SireCrossData result = null;
            try
            {
                result = await _horseService.GetSiresCrossData(maleId1, maleId2);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("wildcard1/search")]
        public async Task<IActionResult> HorsesWildcard1Search(int horse1Id, int horse2Id)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                result = await _horseService.GetHorsesForWildcard1Search(horse1Id, horse2Id);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("wildcard2/search")]
        public async Task<IActionResult> HorsesWildcard2Search(int horse1Id, int? horse2Id, int horse3Id, int? horse4Id)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                result = await _horseService.GetHorsesForWildcard2Search(horse1Id, horse2Id, horse3Id, horse4Id);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("wildcard/query_position")]
        public async Task<IActionResult> HorsesWildcardQueryByPosition(string searches)
        {
            IEnumerable<HorseDTO> result = null;
            try
            {
                var searchesDict = JsonConvert.DeserializeObject<Dictionary<int, int>>(searches);
                result = await _horseService.GetHorsesForWildcardQueryByPosition(searchesDict);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("family_stake/search")]
        public async Task<IActionResult> HorsesForFamilyStakeSearch(int femaleId, int gen)
        {
            IEnumerable<HorseHeirarchy> result = null;
            try
            {
                result = await _horseService.GetHorsesForFamilyStakeSearch(femaleId, gen);

                if (result == null)
                    return BadRequest(new { message = "No horses available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("family_line/search")]
        public async Task<IActionResult> HorseHierarchyForFamilyLineSearch(int femaleId)
        {
            try
            {
                var result = await _horseService.GetHorseHierarchyDataForFemaleLineSearch(femaleId);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }

        [HttpGet("mtdna_lookup/{haploGroupId}")]
        public async Task<IActionResult> HorsesForMtDNALookup(int haploGroupId)
        {
            try
            {
                var result = await _horseService.GetHorsesForMtDNALookup(haploGroupId);

                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}