using Microsoft.AspNetCore.Mvc;
using Services;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDetailService _userDetailService;

        public UserController(UserDetailService userDetailService)
        {
            _userDetailService = userDetailService;
        }



    }
}
