using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDetailController : ControllerBase
    {
        private readonly UserDetailService _userDetailService;

        public UserDetailController(UserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }

        [HttpGet]
        public ActionResult<PaginatedList<UserDetail>> GetPaginatedUserDetails(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedUserDetails = _userDetailService.GetPaginatedUsers(searchQuery, sortBy, pageIndex, pageSize);
            return Ok(paginatedUserDetails);
        }

        [HttpGet("{id}")]
        public ActionResult<UserDetail> GetUserDetailById(string id)
        {
            var userDetail = _userDetailService.GetUserById(id);
            return Ok(userDetail);
        }

        [HttpPut("{id}")]
        public ActionResult UpdateUserDetail([FromBody] UserDetailModel userDetail, string id)
        {
            _userDetailService.UpdateUserDetail(userDetail, id);
            return Ok();
        }
    }
}
