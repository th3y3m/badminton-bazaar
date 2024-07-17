using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        
        private readonly NewsService _newsService;

        public NewsController(NewsService newsService)
        {
            _newsService = newsService;
        }

        [HttpGet]
        public ActionResult GetPaginatedNews(
            [FromQuery] bool? status,
            [FromQuery] bool? isHomePageBanner,
            [FromQuery] bool? isHomePageSlideShow,
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "publicationdate_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedeNews = _newsService.GetPaginatedNews(searchQuery, sortBy, isHomePageBanner, isHomePageSlideShow, status, pageIndex, pageSize);
            return Ok(paginatedeNews);
        }

        [HttpGet("{id}")]
        public ActionResult GetNewsById(string id)
        {
            var news = _newsService.GetNewsById(id);
            return Ok(news);
        }

        [HttpPost]
        public ActionResult AddNews([FromBody] NewsModel newsModel)
        {
            var news = _newsService.AddNews(newsModel);
            return Ok(news);
        }

        [HttpPut]
        public ActionResult UpdateNews([FromBody] NewsModel newsModel, [FromQuery] string id)
        {
            var updatedNews = _newsService.UpdateNews(id, newsModel);
            return Ok(updatedNews);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteNews(string id)
        {
            _newsService.DeleteNews(id);
            return Ok();
        }

    }
}
