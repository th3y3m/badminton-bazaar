using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PriceFactorController : ControllerBase
    {
        private readonly IPriceFactorService _priceFactorService;

        public PriceFactorController(IPriceFactorService priceFactorService)
        {
            _priceFactorService = priceFactorService;
        }

        [HttpGet("{priceFactorId}")]
        public async Task<IActionResult> GetPriceFactorAsync(string priceFactorId)
        {
            try
            {
                var priceFactor = await _priceFactorService.GetPriceFactorAsync(priceFactorId);
                return Ok(priceFactor);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllPriceFactorsAsync()
        {
            try
            {
                var priceFactors = await _priceFactorService.GetAllPriceFactorsAsync();
                return Ok(priceFactors);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddPriceFactorAsync([FromBody] PriceFactor priceFactor)
        {
            try
            {
                await _priceFactorService.AddPriceFactorAsync(priceFactor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePriceFactorAsync([FromBody] PriceFactor priceFactor)
        {
            try
            {
                await _priceFactorService.UpdatePriceFactorAsync(priceFactor);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{priceFactorId}")]
        public async Task<IActionResult> DeletePriceFactorAsync(string priceFactorId)
        {
            try
            {
                await _priceFactorService.DeletePriceFactorAsync(priceFactorId);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
