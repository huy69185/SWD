using AuthenticationApi.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Services
{
    public class EmailService(IConfiguration configuration) : IEmailService
    {
        private readonly string _smtpHost = configuration["Smtp:Host"] ?? throw new InvalidOperationException("SmtpHost configuration is not set.");
        private readonly int _smtpPort = int.Parse(configuration["Smtp:Port"] ?? throw new InvalidOperationException("SmtpPort configuration is not set."));
        private readonly string _smtpUser = configuration["Smtp:Username"] ?? throw new InvalidOperationException("SmtpUser configuration is not set.");
        private readonly string _smtpPass = configuration["Smtp:Password"] ?? throw new InvalidOperationException("SmtpPass configuration is not set.");

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpClient = new SmtpClient(_smtpHost)
            {
                Port = _smtpPort,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser, "Growth Tracking System"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}