using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pedigree.App.Requests;
using Pedigree.Core.Data.DTO;
using Pedigree.Core.Service.Interface;

namespace Pedigree.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RelationshipsController : ControllerBase
    {
        private IRelationshipService _relationshipService;
        private IHorseService _horseService;
        private IValidationService _validationService;

        public RelationshipsController(IHorseService horseService, IRelationshipService relationshipService, IValidationService validationService)
        {
            _horseService = horseService;
            _relationshipService = relationshipService;
            _validationService = validationService;
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RelationshipDTO relationship)
        {   
            try
            {
                // Check if relationship exists for parent 
                var exists = await _relationshipService.GetByOidComoboAsync(relationship.HorseOId, relationship.ParentType);
                if (exists == null)
                {
                    await _relationshipService.CreateRelationshipAsync(relationship); // create relationship we are all good
                }
                else
                {
                    exists.ParentOId = relationship.ParentOId;
                    await _relationshipService.UpdatRelationshipAsync(exists);
                }

                if (relationship.ParentType.Equals("Mother"))
                {
                    await _horseService.PickupParentData(relationship.ParentOId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
            return Ok();
        }

        [HttpDelete("mapping")]
        public async Task<IActionResult> DeleteRelationshipByOId([FromBody] RelationshipOIdRequest request)
        {
            try
            {
                var deleted = await _relationshipService.DeleteByOidComoboAsync(request.HorseOId, request.ParentOId);
                if (!deleted)
                {
                    return BadRequest("Unable to remove relationship at the moment.");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRelationshipByParentType([FromBody] RelationshipParentTypeRequest request)
        {
            try
            {
                var deleted = await _relationshipService.DeleteByParentTypeAsync(request.HorseOId, request.ParentType);
                if (!deleted)
                {
                    return BadRequest("Unable to remove relationship at the moment.");
                }

                if (request.ParentType.Equals("Mother"))
                {
                    await _horseService.RemoveParentData(request.HorseOId);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> Update([FromBody] RelationshipDTO relationship)
        {
            try
            {
                await _relationshipService.UpdatRelationshipAsync(relationship);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        [HttpDelete("Id")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _relationshipService.DeleteRelationshipAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok();
        }

        #region Heirarchy Tabs
        [HttpGet("offsprings/{id}")]
        public async Task<IActionResult> GetOffsprings(int id)
        {
            try
            {
                var result = await _relationshipService.GetHorseOffspringsAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("siblings/{id}")]
        public async Task<IActionResult> GetSiblings(int id)
        {
            try
            {
                var result = await _relationshipService.GetHorseSibilingsAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("femaletail/{id}")]
        public async Task<IActionResult> GetFemaleTail(int id)
        {
            try
            {
                var result = await _relationshipService.GetHorseFemaleTailAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("unique_ancestors/{horseId}")]
        public async Task<IActionResult> GetUniqueAncestors(int horseId)
        {
            try
            {
                var result = await _relationshipService.GetUniqueAncestorsAsync(horseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("unique_ancestors_hypothetical")]
        public async Task<IActionResult> GetUniqueAncestorsForHypothetical(int maleHorseId, int femaleHorseId)
        {
            try
            {
                var result = await _relationshipService.GetUniqueAncestorsAsync(maleHorseId, femaleHorseId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("inbreedings/{horseOId}")]
        public async Task<IActionResult> GetInbreedings(string horseOId, string sort, string order, int page, int size)
        {
            try
            {
                var result = await _relationshipService.GetInbreedingsAsync(horseOId, sort, order, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("grandparents")]
        public async Task<IActionResult> GetGrandparents(string sort, string order, int page, int size)
        {
            try
            {
                var result = await _relationshipService.GetGrandparents(sort, order, page, size);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}