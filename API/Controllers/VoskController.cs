using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NAudio.Wave;
using Services.Interface;
using Services.Models;
using Vosk;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoskController : ControllerBase
    {
        public readonly IVoskService _voskService;   

        public VoskController(IVoskService voskService)
        {
            _voskService = voskService;
        }

        [HttpPost("transcribe")]
        public async Task<IActionResult> TranscribeSpeechFromFile([FromForm] TranscribeSpeechFromFileRequest audio)
        {
            try
            {
                if (audio.Audio == null || audio.Audio.Length == 0)
                    return BadRequest(new { success = false, error = "No audio file provided." });

                var transcription = await _voskService.ConvertSpeechToTextAsyncFromFile(audio.Audio);

                return Ok(new { success = true, transcription });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
