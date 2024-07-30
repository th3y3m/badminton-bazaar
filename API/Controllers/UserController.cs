using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedProducts(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] bool status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedProducts = await _userService.GetPaginatedUsers(searchQuery, sortBy, status, pageIndex, pageSize);
                return Ok(paginatedProducts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated users: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var identityUser = await _userService.GetUserById(id);
                return Ok(identityUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user by ID: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            try
            {
                await _userService.DeleteUser(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting user: {ex.Message}");
            }
        }

        [HttpPut("ban/{id}")]
        public async Task<IActionResult> BanUser(string id)
        {
            try
            {
                await _userService.BanUser(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error banning user: {ex.Message}");
            }
        }

        [HttpPut("unban/{id}")]
        public async Task<IActionResult> UnbanUser(string id)
        {
            try
            {
                await _userService.UnbanUser(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error unbanning user: {ex.Message}");
            }
        }
    }
}
