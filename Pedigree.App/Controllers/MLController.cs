using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
    public class MLController : ControllerBase
    {
        private IMLService _mlService;
        public MLController(IMLService mlService)
        {
            _mlService = mlService;
        }

        [HttpPost("retrain_model/{modelId}")]
        public async Task<IActionResult> RetrainModel([FromRoute] int modelId)
        {
            try
            {
                var result = await _mlService.RetrainMLModel(modelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("train_model")]
        public async Task<IActionResult> TrainModel([FromBody] TrainModelDTO data)
        {
            try
            {
                var result = await _mlService.TrainMLModel(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("evaluate_model")]
        public async Task<IActionResult> EvaluateModel([FromBody] EvaluateModelDTO data)
        {
            try
            {
                var result = await _mlService.EvaluateMLModel(data);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("deploy_model/{modelId}")]
        public async Task<IActionResult> DeployModel([FromRoute] int modelId)
        {
            try
            {
                var result = await _mlService.DeployMLModel(modelId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("last_model")]
        public async Task<IActionResult> GetLastModel()
        {
            try
            {
                var result = await _mlService.GetLastMLModel();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("hypothetical_ml_score")]
        public async Task<IActionResult> GetHypotheticalMLScore(int maleHorseId, int femaleHorseId, string features, int modelId)
        {
            try
            {
                var result = await _mlService.GetHypotheticalMLScore(maleHorseId, femaleHorseId, features.Split(','), modelId);
                return Ok(result);
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
