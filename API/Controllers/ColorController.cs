using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly IColorService _colorService;

        public ColorController(IColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedColors(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "color_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedCategories = await _colorService.GetPaginatedColors(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColorById(string id)
        {
            var color = await _colorService.GetById(id);
            return Ok(color);
        }

        [HttpPost]
        public async Task<IActionResult> AddColor([FromBody] string colorModel)
        {
            var color = await _colorService.Add(colorModel);
            return Ok(color);
        }

        [HttpGet("GetColorsOfProduct/{id}")]
        public async Task<IActionResult> GetColorsOfProduct(string id)
        {
            var colors = await _colorService.GetColorsOfProduct(id);
            return Ok(colors);
        }
    }
}
