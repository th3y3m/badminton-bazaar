using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Helper;
using Services.Interface;
using Services.Models;
using Services;
using System.IdentityModel.Tokens.Jwt;
using StackExchange.Redis;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;

namespace API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthService _authenticationService;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthenticationController(IAuthService authenticationService, SignInManager<IdentityUser> signInManager)
        {
            _authenticationService = authenticationService;
            _signInManager = signInManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var response = await _authenticationService.Login(model);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            try
            {
                await _authenticationService.RegisterSystemAccount(model);

                return Ok("Please verify your email address.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //[HttpPost]
        //[Route("google-login")]
        //public async Task<IActionResult> GoogleLogin(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        //    var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
        //    var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
        //    var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        user = new IdentityUser
        //        {
        //            Email = email,
        //            UserName = email,
        //            SecurityStamp = Guid.NewGuid().ToString()
        //        };
        //        await _userManager.CreateAsync(user);
        //        UserDetail userDetail = new UserDetail()
        //        {
        //            UserId = user.Id,
        //            Point = 0,
        //            FullName = name,
        //            ProfilePicture = picture
        //            //Id = user.Id
        //        };
        //        _userDetailService.AddUserDetail(userDetail);
        //        await _userManager.AddToRoleAsync(user, "Customer");

        //    }

        //    var roles = await _userManager.GetRolesAsync(user);
        //    var userRole = roles.FirstOrDefault();

        //    return Ok(new
        //    {
        //        Token = GenerateJWT.GenerateToken(user, userRole)
        //    });
        //}

        [HttpGet("signin-google")]
        public IActionResult SignInWithGoogle()
        {
            try
            {
                var redirectUrl = Url.Action("GoogleResponse", "Authentication", null, Request.Scheme);
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(GoogleDefaults.AuthenticationScheme, redirectUrl);
                return Challenge(properties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

                if (result == null || !result.Succeeded)
                {
                    return BadRequest("External authentication error");
                }

                var response = await _authenticationService.HandleExternalLoginProviderCallBack(result);

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("signin-facebook")]
        public IActionResult SignInWithFacebook()
        {
            try
            {
                var redirectUrl = Url.Action("FacebookResponse", "Authentication");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(FacebookDefaults.AuthenticationScheme, redirectUrl);
                return Challenge(properties, FacebookDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("facebook/callback")]
        public async Task<IActionResult> FacebookResponse()
        {
            try
            {
                var result = await HttpContext.AuthenticateAsync(FacebookDefaults.AuthenticationScheme);

                if (result == null || !result.Succeeded)
                {
                    return BadRequest("External authentication error");
                }

                var response = await _authenticationService.HandleExternalLoginProviderCallBack(result);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        ////Facebook Login
        //[HttpPost]
        //[Route("facebook-login")]
        //public async Task<IActionResult> FacebookLogin(string token)
        //{
        //    var handler = new JwtSecurityTokenHandler();
        //    var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
        //    var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
        //    var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
        //    var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        user = new IdentityUser
        //        {
        //            Email = email,
        //            UserName = email,
        //            SecurityStamp = Guid.NewGuid().ToString()
        //        };
        //        await _userManager.CreateAsync(user);
        //        UserDetail userDetail = new UserDetail()
        //        {
        //            UserId = user.Id,
        //            Point = 0,
        //            FullName = name,
        //            ProfilePicture = picture
        //        };
        //        _userDetailService.AddUserDetail(userDetail);
        //        await _userManager.AddToRoleAsync(user, "Customer");

        //    }

        //    var roles = await _userManager.GetRolesAsync(user);
        //    var userRole = roles.FirstOrDefault();

        //    return Ok(new
        //    {
        //        Token = GenerateJWT.GenerateToken(user, userRole)
        //    });
        //}

        [HttpGet]
        [Route("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            try
            {
                await _authenticationService.VerifyEmail(email, token);
                return Ok(new ResponseModel { Status = "Success", Message = "Email verified successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest model)
        {
            try
            {
                await _authenticationService.ChangePassword(model);
                return Ok(new ResponseModel { Status = "Success", Message = "Password changed successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("forget-password")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordModel model)
        {
            try
            {
                await _authenticationService.ForgetPassword(model);
                return Ok(new ResponseModel { Status = "Success", Message = "Reset password link has been sent to your email address." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest model)
        {
            try
            {
                await _authenticationService.ResetPassword(model);
                return Ok(new ResponseModel { Status = "Success", Message = "Password reset successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest model)
        {
            try
            {
                var response = await _authenticationService.GenerateRefreshToken(model);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Only send cookie over HTTPS
                    SameSite = SameSiteMode.Strict, // Prevent cross-site requests
                    Expires = DateTime.UtcNow.AddDays(30) // Set cookie expiration
                };

                Response.Cookies.Append("RefreshToken", response.RefreshToken, cookieOptions);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("link-external-login")]
        public async Task<IActionResult> LinkExternalLogin([FromBody] AuthenticateResult result)
        {
            try
            {
                await _authenticationService.LinkExternalLogin(result);
                return Ok(new ResponseModel { Status = "Success", Message = "External login linked successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("unlink-external-login")]
        public async Task<IActionResult> UnlinkExternalLogin([FromBody] string email, [FromBody] string provider)
        {
            try
            {
                await _authenticationService.UnlinkExternalLogin(email, provider);
                return Ok(new ResponseModel { Status = "Success", Message = "External login unlinked successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        [Route("create-password-for-external-login")]
        public async Task<IActionResult> CreatePasswordForExternalLogin([FromBody] CreatePasswordRequest model)
        {
            try
            {
                await _authenticationService.CreatePasswordForExternalLogin(model);
                return Ok(new ResponseModel { Status = "Success", Message = "Password created successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = ex.Message });
            }
        }
    }
}
