﻿using BusinessObjects;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Helper;
using Services.Interface;
using Services.Models;
using System.IdentityModel.Tokens.Jwt;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        //private readonly MailService _mailService;
        private readonly IUserDetailService _userDetailService;

        public AuthenticationController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IUserService userService, IUserDetailService userDetailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _userDetailService = userDetailService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Email or password is empty." });
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
                            Id = user.Id,
                            Token = GenerateJWT.GenerateToken(user, userRole),
                            Role = userRole
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
                        await _userManager.UpdateAsync(user);
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
                await _roleManager.CreateAsync(new IdentityRole("Customer"));


            await _userManager.AddToRoleAsync(user, "Customer");

            UserDetail userDetail = new UserDetail()
            {
                UserId = user.Id,
                Point = 0,
                FullName = model.FullName,
                ProfilePicture = "https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/OIP.jpeg?alt=media&token=60195a0a-2fd6-4c66-9e3a-0f7f80eb8473"
            };
            await _userDetailService.AddUserDetail(userDetail);

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        } 
        
        [HttpPost]
        [Route("registerAdmin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
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

            if (!await _roleManager.RoleExistsAsync("Admin"))
                await _roleManager.CreateAsync(new IdentityRole("Admin"));


            await _userManager.AddToRoleAsync(user, "Admin");

            UserDetail userDetail = new UserDetail()
            {
                UserId = user.Id,
                Point = 0,
                FullName = model.FullName,
                ProfilePicture = "https://firebasestorage.googleapis.com/v0/b/storage-8b808.appspot.com/o/OIP.jpeg?alt=media&token=60195a0a-2fd6-4c66-9e3a-0f7f80eb8473"
            };
            await _userDetailService.AddUserDetail(userDetail);

            return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        }

        //[HttpPost]
        //[Route("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByEmailAsync(model.Email);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new IdentityUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Email
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
        //    }

        //    if (!await _roleManager.RoleExistsAsync("Customer"))
        //    {
        //        var role = new IdentityRole("Customer");
        //        await _roleManager.CreateAsync(role);
        //    }

        //    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //    var base64 = Encoding.UTF8.GetBytes(token);
        //    var encodeToken = WebEncoders.Base64UrlEncode(base64);
        //    Console.WriteLine("code đầu: " + token);
        //    Console.WriteLine("code gửi: " + encodeToken);
        //    var callbackUrl = Url.Action("ResetPassword", "Authentication", new { token = encodeToken, email = user.Email }, Request.Scheme);

        //    var mailRequest = new MailRequest
        //    {
        //        ToEmail = user.Email,
        //        Subject = "Court Caller Confirmation Email (Register)",
        //        Body = FormEmail.EnailContent(user.Email, callbackUrl)
        //    };
        //    await _mailService.SendEmailAsync(mailRequest);

        //    return Ok(new ResponseModel() { Status = "Success", Message = "Please check email to activate account" });
        //    //await _userManager.AddToRoleAsync(user, "Customer");
        //    //UserDetail userDetail = new UserDetail()
        //    //{
        //    //    UserId = user.Id,
        //    //    Point = 0,
        //    //    FullName = model.FullName,
        //    //    ProfilePicture = $"https://firebasestorage.googleapis.com/v0/b/court-callers.appspot.com/o/user.jpg?alt=media&token=3601d057-9503-4cc8-b203-2eb0b89f900d"

        //    //};
        //    //_userDetailService.AddUserDetail(userDetail);
        //    //return Ok(new ResponseModel() { Status = "Success", Message = "User created successfully!" });
        //}
        //[HttpGet]
        //[Route("confirm-email")]
        //public async Task<IActionResult> ConfirmEmail(string email, string token)
        //{
        //    var user = await _userManager.FindByEmailAsync(email);
        //    if (user == null)
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Invalid email." });
        //    var base64 = WebEncoders.Base64UrlDecode(token);
        //    var decodedToken = Encoding.UTF8.GetString(base64);
        //    Console.WriteLine("code nhận: " + token);
        //    Console.WriteLine("code đã decode: " + decodedToken);

        //    var result = _userManager.ConfirmEmailAsync(user, decodedToken);
        //    if (!result.IsCompleted)
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = "Email confirmation failed." });
        //    await _userManager.AddToRoleAsync(user, "Customer");
        //    UserDetail userDetail = new UserDetail()
        //    {
        //        UserId = user.Id,
        //        Point = 0,
        //        FullName = user.UserName,  // Replace with actual full name if available
        //        ProfilePicture = $"https://firebasestorage.googleapis.com/v0/b/court-callers.appspot.com/o/user.jpg?alt=media&token=3601d057-9503-4cc8-b203-2eb0b89f900d"
        //    };
        //    _userDetailService.AddUserDetail(userDetail);

        //    return Ok(new ResponseModel() { Status = "Success", Message = "Email confirmed successfully!" });

        //}




        //[HttpPost]
        //[Route("register-admin")]
        //public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByEmailAsync(model.Email);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new IdentityUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Email
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
        //    }

        //    if (!await _roleManager.RoleExistsAsync("Admin"))
        //        await _roleManager.CreateAsync(new IdentityRole("Admin"));


        //    await _userManager.AddToRoleAsync(user, "Admin");

        //    UserDetail userDetail = new UserDetail()
        //    {
        //        UserId = user.Id,
        //        Point = 0,
        //        FullName = model.FullName,

        //    };
        //    _userDetailService.AddUserDetail(userDetail);

        //    return Ok(new ResponseModel { Status = "Success", Message = "User created successfully!" });
        //}





        //[HttpPost]
        //[Route("register-staff")]
        //public async Task<IActionResult> RegisterStaff([FromBody] RegisterModel model)
        //{
        //    var userExists = await _userManager.FindByEmailAsync(model.Email);
        //    if (userExists != null)
        //        return StatusCode(StatusCodes.Status500InternalServerError, new ResponseModel { Status = "Error", Message = "User already exists!" });

        //    IdentityUser user = new IdentityUser()
        //    {
        //        Email = model.Email,
        //        SecurityStamp = Guid.NewGuid().ToString(),
        //        UserName = model.Email
        //    };
        //    var result = await _userManager.CreateAsync(user, model.Password);
        //    if (!result.Succeeded)
        //    {
        //        var errors = result.Errors.Select(e => e.Description);
        //        return StatusCode(StatusCodes.Status400BadRequest, new ResponseModel { Status = "Error", Message = string.Join(" ", errors) });
        //    }

        //    if (!await _roleManager.RoleExistsAsync("Staff"))
        //        await _roleManager.CreateAsync(new IdentityRole("Staff"));

        //    await _userManager.AddToRoleAsync(user, "Staff");

        //    UserDetail userDetail = new UserDetail()
        //    {
        //        UserId = user.Id,
        //        Point = 0,
        //        FullName = model.FullName,

        //    };
        //    _userDetailService.AddUserDetail(userDetail);

        //    return Ok(new ResponseModel { Status = "Success", Message = "Staff created successfully!" });
        //}



        [HttpPost]
        [Route("google-login")]
        public async Task<IActionResult> GoogleLogin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
            var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
            var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user);
                UserDetail userDetail = new UserDetail()
                {
                    UserId = user.Id,
                    Point = 0,
                    FullName = name,
                    ProfilePicture = picture
                    //Id = user.Id
                };
                _userDetailService.AddUserDetail(userDetail);
                await _userManager.AddToRoleAsync(user, "Customer");

            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            return Ok(new
            {
                Token = GenerateJWT.GenerateToken(user, userRole)
            });
        }

        //Facebook Login
        [HttpPost]
        [Route("facebook-login")]
        public async Task<IActionResult> FacebookLogin(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            var email = jsonToken.Claims.First(claim => claim.Type == "email").Value;
            var name = jsonToken.Claims.First(claim => claim.Type == "name").Value;
            var picture = jsonToken.Claims.First(claim => claim.Type == "picture").Value;

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new IdentityUser
                {
                    Email = email,
                    UserName = email,
                    SecurityStamp = Guid.NewGuid().ToString()
                };
                await _userManager.CreateAsync(user);
                UserDetail userDetail = new UserDetail()
                {
                    UserId = user.Id,
                    Point = 0,
                    FullName = name,
                    ProfilePicture = picture
                };
                _userDetailService.AddUserDetail(userDetail);
                await _userManager.AddToRoleAsync(user, "Customer");

            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            return Ok(new
            {
                Token = GenerateJWT.GenerateToken(user, userRole)
            });
        }




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
    }
}
