using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedNews(
            [FromQuery] bool? status,
            [FromQuery] bool? isHomePageBanner,
            [FromQuery] bool? isHomePageSlideShow,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "publicationdate_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedeNews = await _newsService.GetPaginatedNews(searchQuery, sortBy, isHomePageBanner, isHomePageSlideShow, status, pageIndex, pageSize);
            return Ok(paginatedeNews);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNewsById(string id)
        {
            var news = await _newsService.GetNewsById(id);
            return Ok(news);
        }

        [HttpPost]
        public async Task<IActionResult> AddNews([FromBody] NewsModel newsModel)
        {
            var news = await _newsService.AddNews(newsModel);
            return Ok(news);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNews([FromBody] NewsModel newsModel, [FromQuery] string id)
        {
            var updatedNews = await _newsService.UpdateNews(id, newsModel);
            return Ok(updatedNews);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(string id)
        {
            await _newsService.DeleteNews(id);
            return Ok();
        }

    }
}
