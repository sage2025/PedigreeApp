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
    public class WeightsController : ControllerBase
    {
        private IWeightService _weightService;
        private IValidationService _validationService;
        public WeightsController(IWeightService weightService, IValidationService validationService)
        {
            _weightService = weightService;
            _validationService = validationService;
        }



        [HttpPost]
        public async Task<IActionResult> AddWeight([FromBody] WeightDTO weight)
        {
            try
            {
                var result = await _weightService.CreateWeightAsync(weight);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{weightId}")]
        public async Task<IActionResult> GetWeight(int weightId)
        {
            WeightDTO result = null;
            try
            {
                result = await _weightService.GetWeightAsync(weightId);
                if (result == null)
                    return BadRequest(new { message = "Weight data now found." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw; // TODO:
            }
            return Ok(result);
        }

        [HttpPut("{weightId}")]
        public async Task<IActionResult> UpdateWeight([FromRoute] int weightId, [FromBody] WeightDTO race)
        {
            try
            {
                race.Id = weightId;
                await _weightService.UpdatWeightAsync(race);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWeights(string q, string sort, string order, int page, int size = 100)
        {
            DataListDTO<WeightDTO> result = null;
            try
            {
                if (!string.IsNullOrEmpty(q))
                {
                    result = await _weightService.SearchWeightsByPageAndTotalAsync(q, page, sort, order, size);
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

        [HttpDelete("{weightId}")]
        public async Task<IActionResult> Delete(int weightId)
        {
            try
            {
                await _weightService.DeleteWeightAsync(weightId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
