using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
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
            var paginatedUserDetails = await _userDetailService.GetPaginatedUsers(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedUserDetails);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetailById(string id)
        {
            var userDetail = await _userDetailService.GetUserById(id);
            return Ok(userDetail);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUserDetail([FromBody] UserDetailModel userDetail, string id)
        {
            await _userDetailService.UpdateUserDetail(userDetail, id);
            return Ok();
        }
    }
}
