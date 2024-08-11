using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using BusinessObjects;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreightPriceController : ControllerBase
    {
        private readonly IFreightPriceService _freightPriceService;

        public FreightPriceController(IFreightPriceService freightPriceService)
        {
            _freightPriceService = freightPriceService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var freightPrices = await _freightPriceService.GetAll();
                return Ok(freightPrices);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var freightPrice = await _freightPriceService.GetById(id);
                return Ok(freightPrice);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] FreightPrice freightPrice)
        {
            try
            {
                await _freightPriceService.Add(freightPrice);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] FreightPrice freightPrice)
        {
            try
            {
                await _freightPriceService.Update(freightPrice);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                await _freightPriceService.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("GetPriceByDistance")]
        public async Task<IActionResult> GetPriceByDistance(decimal km)
        {
            try
            {
                var price = await _freightPriceService.GetPriceByDistance(km);
                return Ok(price);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
