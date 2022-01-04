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
    public class PositionsController : ControllerBase
    {
        private IHorseService _horseService;
        private IPositionService _positionService;
        private IValidationService _validationService;
        public PositionsController(IHorseService horseService, IPositionService positionService, IValidationService validationService)
        {
            _horseService = horseService;
            _positionService = positionService;
            _validationService = validationService;
        }



        [HttpPost]
        public async Task<IActionResult> AddPosition([FromBody] PositionDTO position)
        {
            try
            {
                await _validationService.CheckAvailableHorseInRace(position);
                var result = await _positionService.CreatePositionAsync(position);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{positionId}")]
        public async Task<IActionResult> UpdatePosition([FromRoute] int positionId, [FromBody] PositionDTO position)
        {
            try
            {
                await _validationService.CheckAvailableHorseInRace(position, positionId);

                position.Id = positionId;
                await _positionService.UpdatPositionAsync(position);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetPositions(int raceId, string q, int page, int size = 100)
        {
            DataListDTO<PositionDTO> result = null;
            try
            {
                result = await _positionService.SearchPositionsByPageAndTotalAsync(raceId, q, page, size);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpDelete("{positionId}")]
        public async Task<IActionResult> DeletePosition(int positionId)
        {
            try
            {
                await _positionService.DeletePositionAsync(positionId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
