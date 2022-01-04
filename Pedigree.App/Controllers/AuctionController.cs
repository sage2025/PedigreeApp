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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Pedigree.App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuctionController : ControllerBase
    {
        private IAuctionService _auctionService;
        public AuctionController(IAuctionService auctionService)
        {
            _auctionService = auctionService;
        }

        [HttpGet("add_auction")]
        public async Task<IActionResult> AddAuction(string date, string name)
        {
            try
            {
                var result = await _auctionService.AddAuction(date, name);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("add_horse")]
        public async Task<IActionResult> AddHorse(int auctionId, int number, string name, string type, int yob, string sex, string country, string fatherName, string motherName)
        {
            try
            {
                var result = await _auctionService.AddHorse(auctionId, number, name, type, yob, sex, country, fatherName, motherName);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("add_auction_details")]
        public async Task<IActionResult> AddAuctionDetails(AuctionDetailDTO[] details)
        {
            try
            {
                var result = await _auctionService.AddAuctionDetails(details);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get_current_sales")]
        public async Task<IActionResult> GetAuctions()
        {
            Auction[] result;
            try
            {
                result = await _auctionService.GetAuctions();
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete_auction")]
        public async Task<IActionResult> DeleteAcution(int auctionId)
        {
            try
            {
                await _auctionService.DeleteAuction(auctionId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("delete_auction_detail")]
        public async Task<IActionResult> DeleteAuctionDetail(int auctionDetailId)
        {
            try
            {
                await _auctionService.DeleteAuctionDetail(auctionDetailId);
                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("check_pedigcomp")]
        public async Task<IActionResult> CheckPedigComp(int horseId)
        {
            try
            {
                var result = await _auctionService.CheckPedigComp(horseId);
                return Ok(result);
            } 
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get_auction_detail")]
        public async Task<IActionResult> GetAuctionDetail(int auctionId)
        {
            AuctionDetail[] result;
            try
            {
                result = await _auctionService.GetAuctionDetail(auctionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("get_auction")]
        public async Task<IActionResult> GetAuction(int auctionId)
        {
            Auction result;
            try
            {
                result = await _auctionService.GetAuction(auctionId);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("get_mtDNAHap")]
        public async Task<IActionResult> GetmtDNAHap(string motherName)
        {
            MtDNAHap result = null;
            try
            {
                result = await _auctionService.GetMtDNAHap(motherName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
