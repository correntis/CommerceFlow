using AuthService.Infrastructure.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using AuthService.Infrastructure.Abstractions;

namespace AuthService.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IOptions<EmailOptions> _options;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IOptions<EmailOptions> options,
            ILogger<EmailService> logger
            )
        {
            _options = options;
            _logger = logger;
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
                    From = new MailAddress(username, _options.Value.SenderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smptClient.SendMailAsync(mailMessage);

                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.ToString());
                return false;
            }
        }
    }
}
