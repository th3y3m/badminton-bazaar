using BusinessObjects;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using Services;
using Services.Helper;
using Services.Model;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserService _userService;
        private readonly UserDetailService _userDetailService;

        public AuthenticationController(UserService userService, UserDetailService userDetailService)
        {
            _userService = userService;
            _userDetailService = userDetailService;
        }

        [HttpGet]

    }
}
