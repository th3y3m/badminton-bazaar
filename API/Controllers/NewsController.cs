using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;
using System;
using System.Threading.Tasks;

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
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetPaginatedNews(
            [FromQuery] bool? status,
            [FromQuery] bool? isHomePageBanner,
            [FromQuery] bool? isHomePageSlideShow,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "publicationdate_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedeNews = await _newsService.GetPaginatedNews(searchQuery, sortBy, isHomePageBanner, isHomePageSlideShow, status, pageIndex, pageSize);
                return Ok(paginatedeNews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> GetNewsById(string id)
        {
            try
            {
                var news = await _newsService.GetNewsById(id);
                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNews([FromBody] NewsModel newsModel)
        {
            try
            {
                var news = await _newsService.AddNews(newsModel);
                return Ok(news);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateNews([FromBody] NewsModel newsModel, [FromQuery] string id)
        {
            try
            {
                var updatedNews = await _newsService.UpdateNews(id, newsModel);
                return Ok(updatedNews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNews(string id)
        {
            try
            {
                await _newsService.DeleteNews(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("AddAViewUnit/{id}")]
        public async Task<IActionResult> AddAViewUnit(string id)
        {
            try
            {
                await _newsService.AddAViewUnit(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
