using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("google-flan-t5-large")]
        public async Task<IActionResult> GetResponseAsyncUsingGoogleFlanT5Large([FromBody] string userMessage)
        {
            try
            {
                var suggestion = await _chatService.GetResponseAsyncUsingGoogleFlanT5Large(userMessage);
                return Ok(suggestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
