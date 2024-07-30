using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly ISizeService _sizeService;

        public SizeController(ISizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedSizes(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "size_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedSizes = await _sizeService.GetPaginatedSizes(searchQuery, sortBy, pageIndex, pageSize);
                return Ok(paginatedSizes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated sizes: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(string id)
        {
            try
            {
                var size = await _sizeService.GetById(id);
                return Ok(size);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving size by ID: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddSize([FromBody] string sizeModel)
        {
            try
            {
                var size = await _sizeService.Add(sizeModel);
                return Ok(size);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding size: {ex.Message}");
            }
        }

        [HttpGet("GetSizesOfProduct/{id}")]
        public async Task<IActionResult> GetSizesOfProduct(string id)
        {
            try
            {
                var sizes = await _sizeService.GetSizesOfProduct(id);
                return Ok(sizes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving sizes of product: {ex.Message}");
            }
        }
    }
}
