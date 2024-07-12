using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.Helper;
using Services.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<PaginatedList<IdentityUser>> GetPaginatedProducts(
            [FromQuery] string searchQuery = "",
            [FromQuery] string sortBy = "name_asc",
            [FromQuery] bool status = true,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var paginatedProducts = _userService.GetPaginatedUsers(searchQuery, sortBy, status, pageIndex, pageSize);
            return Ok(paginatedProducts);
        }

        [HttpGet("{id}")]
        public ActionResult<IdentityUser> GetUserById(string id)
        {
            var IdentityUser = _userService.GetUserById(id);
            return Ok(IdentityUser);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteUserById(string id)
        {
            _userService.DeleteUser(id);
            return Ok();
        }

        [HttpPut("{id}/ban")]
        public ActionResult BanUser(string id)
        {
            _userService.BanUser(id);
            return Ok();
        }

        [HttpPut("{id}/unban")]
        public ActionResult UnbanUser(string id)
        {
            _userService.UnbanUser(id);
            return Ok();
        }
    }
}
