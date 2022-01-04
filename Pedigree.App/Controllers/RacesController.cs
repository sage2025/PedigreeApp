using Microsoft.AspNetCore.Mvc;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pedigree.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RacesController : ControllerBase
    {
        private IRaceService _raceService;
        private IValidationService _validationService;
        public RacesController(IRaceService raceService, IValidationService validationService)
        {
            _raceService = raceService;
            _validationService = validationService;
        }



        [HttpPost]
        public async Task<IActionResult> AddRace([FromBody] RaceDTO race)
        {
            try
            {
                await _validationService.CheckRaceDuplicate(race);
                var result = await _raceService.CreateRaceAsync(race);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{raceId}")]
        public async Task<IActionResult> GetRace(int raceId)
        {
            RaceDTO result = null;
            try
            {
                result = await _raceService.GetRaceAsync(raceId);
                if (result == null)
                    return BadRequest(new { message = "Race data now found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw; // TODO:
            }
            return Ok(result);
        }

        [HttpPut("{raceId}")]
        public async Task<IActionResult> UpdateRace([FromRoute] int raceId, [FromBody] RaceDTO race)
        {
            try
            {
                await _validationService.CheckRaceDuplicate(race, raceId);
                
                race.Id = raceId;
                await _raceService.UpdatRaceAsync(race);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetRaces(string q, string sort, string order, int page, int size = 100)
        {
            DataListDTO<RaceDTO> result = null;
            try
            {
                if (!string.IsNullOrEmpty(q))
                {
                    result = await _raceService.SearchRacesByPageAndTotalAsync(q, page, sort, order, size);
                }

                if (result == null)
                    return BadRequest(new { message = "No races available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("hasresult/{raceId}")]
        public async Task<IActionResult> HasResult(int raceId)
        {
            var result = false;
            try
            {
                result = await _raceService.HasResult(raceId);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(result);
        }

        [HttpDelete("{raceId}")]
        public async Task<IActionResult> Delete(int raceId)
        {
            try
            {
                await _raceService.DeleteRaceAsync(raceId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
