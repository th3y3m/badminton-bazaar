using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ColorController : ControllerBase
    {
        private readonly ColorService _colorService;

        public ColorController(ColorService colorService)
        {
            _colorService = colorService;
        }

        [HttpGet]
        public ActionResult GetPaginatedColors(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "color_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedCategories = _colorService.GetPaginatedColors(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedCategories);
        }

        [HttpGet("{id}")]
        public ActionResult GetColorById(string id)
        {
            var color = _colorService.GetById(id);
            return Ok(color);
        }

        [HttpPost]
        public ActionResult AddColor([FromBody] string colorModel)
        {
            var color = _colorService.Add(colorModel);
            return Ok(color);
        }
    }
}
