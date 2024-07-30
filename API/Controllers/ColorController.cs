using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using System;
using System.Threading.Tasks;

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
            try
            {
                var paginatedCategories = await _colorService.GetPaginatedColors(searchQuery, sortBy, pageIndex, pageSize);
                return Ok(paginatedCategories);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetColorById(string id)
        {
            try
            {
                var color = await _colorService.GetById(id);
                return Ok(color);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddColor([FromBody] string colorModel)
        {
            try
            {
                var color = await _colorService.Add(colorModel);
                return Ok(color);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetColorsOfProduct/{id}")]
        public async Task<IActionResult> GetColorsOfProduct(string id)
        {
            try
            {
                var colors = await _colorService.GetColorsOfProduct(id);
                return Ok(colors);
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
