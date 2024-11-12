using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IAIService _aiService;

        public AIController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("local-text-ai")]
        public async Task<IActionResult> GetResponseAsyncUsingLocalTextGenerationAI([FromBody] string userMessage)
        {
            try
            {
                var suggestion = await _aiService.GetResponseAsyncUsingLocalTextGenerationAI(userMessage);
                return Ok(suggestion);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("local-image-ai")]
        public async Task<IActionResult> GetResponseAsyncUsingLocalImageGenerationAI([FromBody] string userMessage)
        {
            try
            {
                // Call the AI service to get the base64-encoded image
                var base64Image = await _aiService.GetResponseAsyncUsingLocalImageGenerationAI(userMessage);

                // Check if we got a valid image
                if (string.IsNullOrEmpty(base64Image) || !IsValidBase64(base64Image))
                {
                    return BadRequest("Failed to generate the image or received invalid base64 data.");
                }

                // Decode the base64 string into byte array
                var imageBytes = Convert.FromBase64String(base64Image);

                // Return the image as a FileResult (image/png in this case)
                return File(imageBytes, "image/png");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private bool IsValidBase64(string base64)
        {
            try
            {
                Convert.FromBase64String(base64);  // This will throw if the string is invalid
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
