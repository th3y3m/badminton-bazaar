using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Interface;
using Services.Models;

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
            var paginatedProducts = await _userService.GetPaginatedUsers(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var IdentityUser = await _userService.GetUserById(id);
            return Ok(IdentityUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserById(string id)
        {
            await _userService.DeleteUser(id);
            return Ok();
        }

        [HttpPut("ban/{id}")]
        public async Task<IActionResult> BanUser(string id)
        {
            await _userService.BanUser(id);
            return Ok();
        }

        [HttpPut("unban/{id}")]
        public async Task<IActionResult> UnbanUser(string id)
        {
            await _userService.UnbanUser(id);
            return Ok();
        }
    }
}
