using AuthService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AuthService.Infrastructure.Abstractions;

namespace AuthService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailOptions> _options;

        public EmailService(
            IOptions<EmailOptions> options
            )
        {
            _options = options;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var host = _options.Value.Host;
                var username = _options.Value.UserName;
                var port = _options.Value.Port;
                var password = _options.Value.Password;

                var smptClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_options.Value.SenderName, _options.Value.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smptClient.SendMailAsync(mailMessage);

                return true;
            }
            catch
            {

                return false;
            }
        }
    }
}
