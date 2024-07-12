using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SizeController : Controller
    {
        private readonly SizeService _sizeService;

        public SizeController(SizeService sizeService)
        {
            _sizeService = sizeService;
        }

        [HttpGet]
        public ActionResult GetPaginatedSizes(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "size_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedSizes = _sizeService.GetPaginatedSizes(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedSizes);
        }

        [HttpGet("{id}")]
        public ActionResult GetSizeById(string id)
        {
            var size = _sizeService.GetById(id);
            return Ok(size);
        }

        [HttpPost]
        public ActionResult AddSize([FromBody] string sizeModel)
        {
            var size = _sizeService.Add(sizeModel);
            return Ok(size);
        }
    }
}
