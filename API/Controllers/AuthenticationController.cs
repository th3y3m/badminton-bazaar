using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Repositories;
using Services;
using Services.Helper;
using Services.Models;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        private readonly MailService _mailService;
        private readonly UserDetailService _userDetailService;
        private readonly IConfiguration _configuration;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, MailService mailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _mailService = mailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            //if (ValidatePassword.ValidatePass(model.Password) == false)
            //    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Password format is incorrect." });
            if (model.Email == null || model.Password == null || model.Email == "" || model.Password == "")
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "Email or password is empty." });
            //var ip = Utils.GetIpAddress(HttpContext);
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return
                    StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User not found!" });
            }

            //check if user is banned
            //if (BanList.BannedUsers.Contains(ip) || userDetail.Status == false)
            if (user.LockoutEnabled == false)
            {
                return
                    StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User is banned!" });
            }
            else
            {
                try
                {
                    if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var userRole = roles.FirstOrDefault();
                        return Ok(new
                        {
                            Token = GenerateJWT.GenerateToken(user, userRole)
                        });
                    }
                    else
                    {
                        user.AccessFailedCount++;
                        if (user.AccessFailedCount == 5)
                        {
                            //BanList.BannedUsers.Add(ip);
                            user.LockoutEnabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    return BadRequest();
                }
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

            IdentityUser user = new IdentityUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = model.Email
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
            }

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                var role = new IdentityRole("Customer");
                await _roleManager.CreateAsync(role);
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var base64 = Encoding.UTF8.GetBytes(token);
            var encodeToken = WebEncoders.Base64UrlEncode(base64);
            Console.WriteLine("code đầu: " + token);
            Console.WriteLine("code gửi: " + encodeToken);
            var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token = encodeToken, email = user.Email }, Request.Scheme);

            var mailRequest = new MailRequest
            {
                ToEmail = user.Email,
                Subject = "Court Caller Confirmation Email (Register)",
                Body = FormEmail.EnailContent(user.Email, callbackUrl)
            };
            await _mailService.SendEmailAsync(mailRequest);

            return Ok(new ResponseModel() { Status = "Success", Message = "Please check email to activate account" });
            //await _userManager.AddToRoleAsync(user, "Customer");
            //UserDetail userDetail = new UserDetail()
            //{
            //    UserId = user.Id,
            //    Point = 0,
            //    FullName = model.FullName,
            //    ProfilePicture = $"https://firebasestorage.googleapis.com/v0/b/court-callers.appspot.com/o/user.jpg?alt=media&token=3601d057-9503-4cc8-b203-2eb0b89f900d"

            //};
            //_userDetailService.AddUserDetail(userDetail);
            //return Ok(new ResponseModel() { Status = "Success", Message = "User created successfully!" });
        }

    }
}
