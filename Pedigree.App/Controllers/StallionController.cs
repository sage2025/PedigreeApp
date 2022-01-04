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
    public class StallionController : ControllerBase
    {
        private IStallionRatingService _stallionRatingService;
        public StallionController(IStallionRatingService stallionRatingService, IValidationService validationService)
        {
            _stallionRatingService = stallionRatingService;
        }
        

        [HttpGet("stallion_ratings")]
        public async Task<IActionResult> GetStallionRatings(string t, string q, string sort, string order, int page, int size = 100)
        {
            StallionRatingListDTO result = null;
            try
            {
                result = await _stallionRatingService.GetStallionRatings(t, q, sort, order, page, size);

                if (result == null)
                    return BadRequest(new { message = "No stallion ratings available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }
    }
}
