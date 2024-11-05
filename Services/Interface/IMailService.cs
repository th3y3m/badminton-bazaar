using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendConfirmationEmailAsync(string toMail, string token);
        Task SendForgetPasswordEmailAsync(string toMail, string token);
    }
}
