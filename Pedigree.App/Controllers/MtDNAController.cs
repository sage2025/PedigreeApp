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
    public class MtDNAController : ControllerBase
    {
        private IMtDNAService _mtdnaService;
        public MtDNAController(IMtDNAService mtdnaService, IValidationService validationService)
        {
            _mtdnaService = mtdnaService;
        }

        [HttpPost("haplotypes")]
        public async Task<IActionResult> AddHaploType([FromBody] HaploTypeDTO haploType)
        {
            try
            {
                var result = await _mtdnaService.CreateHaploTypeAsync(haploType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("haplotypes/{typeId}")]
        public async Task<IActionResult> UpdateHaploType([FromRoute] int typeId, [FromBody] HaploTypeDTO dto)
        {
            try
            {
                dto.Id = typeId;
                await _mtdnaService.UpdatHaploTypeAsync(dto);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("haplotypes/{typeId}")]
        public async Task<IActionResult> DeleteHaploType(int typeId)
        {
            try
            {
                await _mtdnaService.DeleteHaploTypeAsync(typeId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("simple_haplogroups")]
        public async Task<IActionResult> GetSimpleHaploGroups()
        {
            HaploGroupDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetSimpleHaploGroups();

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("haplogroups")]
        public async Task<IActionResult> GetHaploGroups()
        {
            HaploGroupDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetHaploGroups();

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("horse_haplogroups/{horseId}")]
        public async Task<IActionResult> GetHorseHaploGroups(int horseId)
        {
            HaploGroupDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetHorseHaploGroups(horseId);

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("hypothetical_horse_haplogroups")]
        public async Task<IActionResult> GetHypotheticalHorseHaploGroups(int maleId, int femaleId)
        {
            HaploGroupDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetHypotheticalHaploGroups(maleId, femaleId);

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpPut("haplogroups/{groupId}/color")]
        public async Task<IActionResult> UpdateHaploGroupColor([FromRoute] int groupId, [FromBody] HaploGroupColorDTO dto)
        {
            try
            {
                await _mtdnaService.UpdateHaploGroupAsync(groupId, dto.Color);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("haplogroups_stallion")]
        public async Task<IActionResult> GetHaploGroupsStallion(int maleId)
        {
            HaploGroupStallionDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetHaploGroupsStallion(maleId);

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpGet("haplogroups_distance")]
        public async Task<IActionResult> GetHaploGroupsDistance()
        {
            HaploGroupDistanceDTO[] result = null;
            try
            {
                result = await _mtdnaService.GetHaploGroupsDistance();

                if (result == null)
                    return BadRequest(new { message = "No haplogroups available at the moment." });
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }

            return Ok(result);
        }

        [HttpPost("flags")]
        public async Task<IActionResult> AddMtDNAFlag([FromBody] MtDNAFlagDTO flagDTO)
        {
            try
            {
                var result = await _mtdnaService.CreateMtDNAFlag(flagDTO);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("flags")]
        public async Task<IActionResult> GetMtDNAFlags()
        {
            try
            {
                var result = await _mtdnaService.GetMtDNAFlags();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("flags/{flagId}")]
        public async Task<IActionResult> DeleteMtDNAFlags(int flagId)
        {
            try
            {
                await _mtdnaService.DeleteMtDNAFlag(flagId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
