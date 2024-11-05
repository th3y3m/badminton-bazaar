using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Services.Helper;
using Services.Interface;
using Services.Models;
using StackExchange.Redis;

namespace Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserService _userService;
        private readonly IMailService _mailService;
        private readonly IUserDetailService _userDetailService;
        private readonly IRedisLock _redisLock;
        private readonly IConnectionMultiplexer _redisConnection;
        private readonly IDatabase _redisDb;
        private readonly AsyncRetryPolicy _dbRetryPolicy;
        private readonly AsyncTimeoutPolicy _dbTimeoutPolicy;
        private readonly AsyncPolicyWrap _dbPolicyWrap;

        public AuthenticationService(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserService userService,
            IMailService mailService,
            IUserDetailService userDetailService,
            IRedisLock redisLock,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
            _mailService = mailService;
            _userDetailService = userDetailService;
            _redisLock = redisLock;
            _redisConnection = connectionMultiplexer;
            _redisDb = _redisConnection.GetDatabase();

            _dbRetryPolicy = Policy.Handle<SqlException>()
                                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                                (exception, timeSpan, retryCount, context) =>
                                {
                                    Console.WriteLine($"[Db Retry] Attempt {retryCount} after {timeSpan} due to: {exception.Message}");
                                });
            _dbTimeoutPolicy = Policy.TimeoutAsync(10, TimeoutStrategy.Optimistic, (context, timeSpan, task) =>
            {
                Console.WriteLine($"[Db Timeout] Operation timed out after {timeSpan}");
                return Task.CompletedTask;
            });
            _dbPolicyWrap = Policy.WrapAsync(_dbRetryPolicy, _dbTimeoutPolicy);
        }

        public async Task<LoginResponse> Login(LoginModel model)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(model.Email));

                if (user == null)
                {
                    throw new ArgumentNullException("User not found!");
                }

                if (user.LockoutEnabled == false)
                {
                    throw new ArgumentException("User is banned!");
                }

                var result = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.CheckPasswordAsync(user, model.Password));

                if (!result)
                {
                    user.AccessFailedCount++;
                    if (user.AccessFailedCount == 5)
                    {
                        user.LockoutEnabled = false;
                    }
                    await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.UpdateAsync(user));

                    throw new ArgumentException("Invalid password!");
                }

                var roles = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.GetRolesAsync(user));
                var userRole = roles.FirstOrDefault();
                var token = GenerateJWT.GenerateToken(user, userRole);
                var refreshToken = Guid.NewGuid().ToString();
                var tokenExpiration = DateTime.Now.AddDays(30);

                var userDetail = await _userDetailService.GetUserById(user.Id);
                userDetail.RefreshToken = refreshToken;
                userDetail.RefreshTokenExpiration = tokenExpiration;
                await _userDetailService.UpdateUserDetail(userDetail);

                return new LoginResponse
                {
                    Id = user.Id,
                    Token = token,
                    Role = userRole,
                    RefreshToken = refreshToken,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error logging in: {ex.Message}");
            }
        }

        public async Task RegisterSystemAccount(RegisterModel model)
        {
            string lockKey = "register_lock_" + model.Email;
            string lockValue = Guid.NewGuid().ToString();

            if (!_redisLock.AcquireLock(lockKey, lockValue))
            {
                throw new Exception("Failed to acquire lock");
            }

            try
            {
                var userExists = await _userManager.FindByEmailAsync(model.Email);

                if (userExists != null && userExists.EmailConfirmed)
                    throw new ArgumentException("User already exists!");

                var refreshToken = Guid.NewGuid().ToString();
                var tokenExpiration = DateTime.Now.AddDays(30);
                string mailToken = null;

                if (userExists == null)
                {
                    var user = new IdentityUser
                    {
                        Email = model.Email,
                        UserName = model.Email,
                        EmailConfirmed = false,
                        PasswordHash = model.ConfirmPassword
                    };

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new ArgumentException("User creation failed: " + errors);
                    }

                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }

                    await _userManager.AddToRoleAsync(user, "Customer");

                    var newUserDetail = new UserDetail
                    {
                        UserId = user.Id,
                        Point = 0,
                        FullName = model.FullName,
                        RefreshToken = refreshToken,
                        RefreshTokenExpiration = tokenExpiration,
                        ProfilePicture = "https://example.com/default-profile.png"
                    };

                    await _userDetailService.AddUserDetail(newUserDetail);

                    mailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                }
                else
                {
                    var userDetail = await _userDetailService.GetUserById(userExists.Id);
                    userDetail.RefreshToken = refreshToken;
                    userDetail.RefreshTokenExpiration = tokenExpiration;

                    await _userDetailService.UpdateUserDetail(userDetail);

                    mailToken = await _userManager.GenerateEmailConfirmationTokenAsync(userExists);
                }

                await _mailService.SendConfirmationEmailAsync(model.Email, mailToken);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error registering user: {ex.Message}");
            }
            finally
            {
                _redisLock.ReleaseLock(lockKey, lockValue);
            }
        }

        public async Task VerifyEmail(string email, string token)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(email));
                if (user == null)
                {
                    throw new ArgumentNullException("User not found!");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new ArgumentException("Email verification failed: " + string.Join(", ", errors));
                }

                user.EmailConfirmed = true;
                await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.UpdateAsync(user));
            }
            catch (Exception ex)
            {
                throw new Exception($"Error verifying email: {ex.Message}");
            }
        }

        public async Task ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(request.Mail));
                if (user == null)
                {
                    throw new ArgumentNullException("User not found!");
                }

                var result = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword));

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new ArgumentException("Password change failed: " + string.Join(", ", errors));
                }

                var req = new MailRequest()
                {
                    ToEmail = user.Email,
                    Subject = "Password Changed",
                    Body = "Your password has been changed successfully"
                };

                await _mailService.SendEmailAsync(req);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error changing password: {ex.Message}");
            }
        }

        public async Task ForgetPassword(ForgetPasswordModel model)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(model.Email));
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return;
                }

                var token = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.GeneratePasswordResetTokenAsync(user));

                await _mailService.SendForgetPasswordEmailAsync(user.Email, token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resetting password: {ex.Message}");
            }
        }

        public async Task ResetPassword(ResetPasswordRequest model)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(model.Email));
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    return;
                }

                var result = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.ResetPasswordAsync(user, model.Token, model.Password));

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    throw new ArgumentException("Password reset failed: " + string.Join(", ", errors));
                }

                var req = new MailRequest()
                {
                    ToEmail = user.Email,
                    Subject = "Password Reset",
                    Body = "Your password has been reset successfully"
                };

                await _mailService.SendEmailAsync(req);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error resetting password: {ex.Message}");
            }
        }

        public async Task<RefreshTokenResponse> GenerateRefreshToken(RefreshTokenRequest request)
        {
            try
            {
                var user = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(request.Email));
                if (user == null)
                {
                    throw new ArgumentNullException("User not found!");
                }

                var userDetail = await _userDetailService.GetUserById(user.Id);
                if (userDetail.RefreshToken != request.RefreshToken || userDetail.RefreshTokenExpiration < DateTime.Now)
                {
                    throw new ArgumentException("Invalid refresh token!");
                }

                var roles = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.GetRolesAsync(user));
                var userRole = roles.FirstOrDefault();
                var token = GenerateJWT.GenerateToken(user, userRole);
                var refreshToken = Guid.NewGuid().ToString();
                var tokenExpiration = DateTime.Now.AddDays(30);

                userDetail.RefreshToken = refreshToken;
                userDetail.RefreshTokenExpiration = tokenExpiration;
                await _userDetailService.UpdateUserDetail(userDetail);

                return new RefreshTokenResponse
                {
                    Token = token,
                    RefreshToken = refreshToken
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error refreshing token: {ex.Message}");
            }
        }

        public async Task<LoginResponse> HandleExternalLoginProviderCallBack(AuthenticateResult authenticateResult)
        {
            try
            {
                var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);
                var phoneNumber = authenticateResult.Principal.FindFirstValue("phone");
                var name = authenticateResult.Principal.FindFirstValue(ClaimTypes.Name);
                var username = authenticateResult.Principal.FindFirstValue("preferred_username");
                var profilePicture = authenticateResult.Principal.FindFirstValue("picture");
                var provider = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier); // Use this for provider login info
                var refreshToken = Guid.NewGuid().ToString();
                var tokenExpiration = DateTime.Now.AddDays(30);

                var existedUser = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.FindByEmailAsync(email));
                var user = new IdentityUser();

                if (existedUser == null)
                {
                    user = new IdentityUser
                    {
                        Email = email,
                        PhoneNumber = phoneNumber,
                        UserName = username,
                        EmailConfirmed = true // Set as confirmed if you trust the external provider
                    };

                    // Create user without a password
                    var result = await _userManager.CreateAsync(user);
                    if (!result.Succeeded)
                    {
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        throw new ArgumentException("User creation failed: " + errors);
                    }

                    // Create and add to the Customer role if not exists
                    if (!await _roleManager.RoleExistsAsync("Customer"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("Customer"));
                    }

                    await _userManager.AddToRoleAsync(user, "Customer");

                    var newUserDetail = new UserDetail
                    {
                        UserId = user.Id,
                        Point = 0,
                        FullName = name,
                        ProfilePicture = profilePicture
                    };

                    await _userDetailService.AddUserDetail(newUserDetail);
                }

                // Link the external login to the user
                var loginInfo = new UserLoginInfo(provider, authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier), provider);
                await _userManager.AddLoginAsync(existedUser ?? user, loginInfo);

                var roles = await _dbPolicyWrap.ExecuteAsync(async () => await _userManager.GetRolesAsync(existedUser ?? user));
                var userRole = roles.FirstOrDefault();
                var token = GenerateJWT.GenerateToken(existedUser ?? user, userRole);

                var userDetail = await _userDetailService.GetUserById((existedUser ?? user).Id);
                userDetail.RefreshToken = refreshToken;
                userDetail.RefreshTokenExpiration = tokenExpiration;
                await _userDetailService.UpdateUserDetail(userDetail);

                return new LoginResponse
                {
                    Id = (existedUser ?? user).Id,
                    Token = token,
                    Role = userRole,
                    RefreshToken = refreshToken,
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error handling external login: {ex.Message}");
            }
        }

    }
}
