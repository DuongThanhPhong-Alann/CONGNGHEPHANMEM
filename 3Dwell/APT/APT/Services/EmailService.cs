using Microsoft.Extensions.Options;
using QLCCCC.Models;
using QLCCCC.Repositories.Interfaces;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;

namespace QLCCCC.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, string? from = null)
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(from ?? _emailSettings.FromAddress, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            mailMessage.To.Add(to);

            using (var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.SmtpUser, _emailSettings.SmtpPass);
                smtpClient.EnableSsl = true;

                try
                {
                    _logger.LogInformation($"Attempting to send email to {to} from {from ?? _emailSettings.FromAddress}");
                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully.");
                }
                catch (SmtpException ex)
                {
                    // Log the SMTP error details
                    _logger.LogError($"SMTP Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
                catch (Exception ex)
                {
                    // Catch any general exception
                    _logger.LogError($"General Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                }
            }
        }
    }
}
