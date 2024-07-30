using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services.Interface;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {
        private readonly IUserDetailService _userDetailService;

        public UserDetailController(IUserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedUserDetails(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var paginatedUserDetails = await _userDetailService.GetPaginatedUsers(searchQuery, sortBy, pageIndex, pageSize);
                return Ok(paginatedUserDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving paginated user details: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetailById(string id)
        {
            try
            {
                var userDetail = await _userDetailService.GetUserById(id);
                return Ok(userDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user detail by ID: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailModel userDetail, string id)
        {
            try
            {
                await _userDetailService.UpdateUserDetail(userDetail, id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user detail: {ex.Message}");
            }
        }
    }
}
