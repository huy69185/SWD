using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace AuthenticationApi.Application.Services
{
    public class NotificationService(IEmailService emailService,
        ISmsService smsService,
        ILogger<NotificationService> logger,
        IHttpContextAccessor contextAccessor) : INotificationService
    {
        public async Task SendRegistrationNotificationAsync(string email, string verifyToken)
        {
            var emailSubject = "Verify Your Email Address";
            var request = contextAccessor.HttpContext?.Request;
            var emailBody = $"Please verify your email by clicking the link: " +
                $"<a href='{Util.GetCurrentDomain(request)}/api/auth/verify-email?token={verifyToken}'>Verify Email</a><br>" +
                $"<p>Your verify token: {verifyToken}</p>";

            await emailService.SendEmailAsync(email, emailSubject, emailBody);
            logger.LogInformation($"Sent email verification to {email}");

            var registrationEmailSubject = "Welcome to Growth Tracking System!";
            var registrationEmailBody = "Your account has been successfully created. Please verify your email to activate your account.";
            await emailService.SendEmailAsync(email, registrationEmailSubject, registrationEmailBody);

            /*if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var sendOtpDTO = new SendOtpDTO(user.PhoneNumber);
                await smsService.SendOtpAsync(sendOtpDTO);
            }*/
        }

        public async Task SendForgotPasswordEmailAsync(string email, string otpCode)
        {
            var subject = "Your Password Reset Code";
            var body = $"Your password reset code is: <strong>{otpCode}</strong>. This code will expire in 1 hour.";
            await emailService.SendEmailAsync(email, subject, body);
            logger.LogInformation($"Sent password reset code to {email}");
        }

        public async Task SendForgotPasswordSmsAsync(string phoneNumber, string otpCode)
        {
            var sendOtpDTO = new SendOtpDTO(phoneNumber);
            await smsService.SendOtpAsync(sendOtpDTO);
            logger.LogInformation($"Sent password reset code to {phoneNumber} via SMS");
        }
    }
}