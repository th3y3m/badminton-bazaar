using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponse> Login(LoginModel request);
        Task RegisterSystemAccount(RegisterModel model);
        Task VerifyEmail(string email, string token);
        Task<LoginResponse> HandleExternalLoginProviderCallBack(AuthenticateResult result);
        Task ChangePassword(ChangePasswordRequest request);
        Task ForgetPassword(ForgetPasswordModel model);
        Task ResetPassword(ResetPasswordRequest model);
        Task<RefreshTokenResponse> GenerateRefreshToken(RefreshTokenRequest request);
        Task LinkExternalLogin(AuthenticateResult authenticateResult);
        Task UnlinkExternalLogin(string email, string provider);
    }
}
