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
                var redirectUrl = Url.Action("GoogleResponse", "Account");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
                return Challenge(properties, "Google");
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
                var result = await HttpContext.AuthenticateAsync("External");

                if (result == null)
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
                var redirectUrl = Url.Action("FacebookResponse", "Account");
                var properties = _signInManager.ConfigureExternalAuthenticationProperties("Facebook", redirectUrl);
                return Challenge(properties, "Facebook");
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
                var result = await HttpContext.AuthenticateAsync("External");

                if (result == null)
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




        ////ForgetPassword
        //[HttpPost]
        //[Route("forget-password")]
        //public async Task<IActionResult> ForgetPassword([FromBody] string email)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User does not exist!" });

        //    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        //    var base64 = Encoding.UTF8.GetBytes(token);
        //    var encodeToken = WebEncoders.Base64UrlEncode(base64);
        //    var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token = encodeToken, email = user.Email }, Request.Scheme);

        //    var mailRequest = new MailRequest
        //    {
        //        ToEmail = user.Email,
        //        Subject = "Court Caller Confirmation Email (Reset Password)",
        //        Body = FormEmail.EnailContent(user.Email, callbackUrl)
        //    };
        //    await _mailService.SendEmailAsync(mailRequest);

        //    return Ok(new ResponseModel { Status = "Success", Message = "Reset password link has been sent to your email address." });
        //}

        //[HttpGet]
        //[Route("reset-password")]
        //public async Task<IActionResult> ResetPassword(string token, string email)
        //{
        //    if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
        //    {
        //        return BadRequest("Invalid password reset token or email.");
        //    }

        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    //var isTokenValid = await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token);
        //    //if (!isTokenValid)
        //    //{
        //    //    return BadRequest("Invalid token.");
        //    //}

        //    // Redirect to the React app's reset password page with token and email
        //    var resetPasswordUrl = $"https://localhost:3000/reset-password?token={token}&email={email}";
        //    return Redirect(resetPasswordUrl);
        //}



        ////ResetPassword
        //[HttpPost]
        //[Route("reset-password")]
        //public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        //{


        //    var user = await _userManager.FindByEmailAsync(model.Email);
        //    if (user == null)
        //        return RedirectToAction("ResetPasswordConfirmation", "Authentication");
        //    var base64 = WebEncoders.Base64UrlDecode(model.Token);
        //    var decodedToken = Encoding.UTF8.GetString(base64);

        //    var resetPassResult = await _userManager.ResetPasswordAsync(user, decodedToken, model.Password);


        //    if (!resetPassResult.Succeeded)
        //    {
        //        foreach (var error in resetPassResult.Errors)
        //        {
        //            ModelState.AddModelError(string.Empty, error.Description);
        //        }
        //        return BadRequest(new ResponseModel { Status = "Error", Message = "Something Wrong, Please Try Again" });
        //    }

        //    return Ok(new ResponseModel { Status = "Complele", Message = "Confirmed" });
        //}

        [HttpPost]
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
    }
}
