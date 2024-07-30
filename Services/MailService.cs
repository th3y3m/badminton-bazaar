﻿using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Services.Interface;
using Services.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Services
{
    internal class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            try
            {
                // Setup mail
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
                email.Subject = mailRequest.Subject;
                var builder = new BodyBuilder();

                // Attachment
                if (mailRequest.Attachments != null)
                {
                    byte[] fileBytes;
                    foreach (var file in mailRequest.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                fileBytes = ms.ToArray();
                            }
                            builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                        }
                    }
                }

                // Body
                builder.HtmlBody = mailRequest.Body;
                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending email: {ex.Message}");
            }
        }

        public async Task SendConfirmationEmailAsync(MailRequest request)
        {
            try
            {
                // Html Mail
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "Templates", "WelcomeTemplate.html");
                string mailText;
                using (var str = new StreamReader(filePath))
                {
                    mailText = await str.ReadToEndAsync();
                }
                mailText = mailText.Replace("[ConfirmLink]", request.Body);

                // Setup email
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
                email.To.Add(MailboxAddress.Parse(request.ToEmail));
                email.Subject = request.Subject;
                var builder = new BodyBuilder
                {
                    HtmlBody = mailText // Using Html file edited instead of request.body
                };
                email.Body = builder.ToMessageBody();

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error sending confirmation email: {ex.Message}");
            }
        }
    }
}
