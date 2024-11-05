using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
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
    }
}
