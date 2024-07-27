using Microsoft.AspNetCore.Mvc;
using Services;
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
            var paginatedSizes = await _sizeService.GetPaginatedSizes(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedSizes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSizeById(string id)
        {
            var size = await _sizeService.GetById(id);
            return Ok(size);
        }

        [HttpPost]
        public async Task<IActionResult> AddSize([FromBody] string sizeModel)
        {
            var size = await _sizeService.Add(sizeModel);
            return Ok(size);
        }

        [HttpGet("GetSizesOfProduct/{id}")]
        public async Task<IActionResult> GetSizesOfProduct(string id)
        {
            var sizes = await _sizeService.GetSizesOfProduct(id);
            return Ok(sizes);
        }
    }
}
