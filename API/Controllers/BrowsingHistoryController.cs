using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrowsingHistoryController : ControllerBase
    {
        private readonly IBrowsingHistoryService _browsingHistoryService;

        public BrowsingHistoryController(IBrowsingHistoryService browsingHistoryService)
        {
            _browsingHistoryService = browsingHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var browsingHistories = await _browsingHistoryService.Get();
                return Ok(browsingHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserHistory(string userId)
        {
            try
            {
                var browsingHistories = await _browsingHistoryService.GetUserHistories(userId);
                return Ok(browsingHistories);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
