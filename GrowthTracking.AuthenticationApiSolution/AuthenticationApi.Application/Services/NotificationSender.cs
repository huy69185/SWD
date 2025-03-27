using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Application.DTOs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace AuthenticationApi.Application.Services
{
    public class NotificationSender
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly ILogger<NotificationSender> _logger;

        public NotificationSender(IUserRepository userRepository, IEmailService emailService, ISmsService smsService, ILogger<NotificationSender> logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _smsService = smsService;
            _logger = logger;
        }

        public async Task SendRegistrationNotificationAsync(AppUserDTO user)
        {
            // Gửi email xác thực
            var emailSubject = "Verify Your Email Address";
            var emailBody = $"Please verify your email by clicking the link: <a href='http://localhost:5002/api/auth/verify-email?token={user.VerificationToken}'>Verify Email</a>";
            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);
            _logger.LogInformation($"Sent email verification to {user.Email}");

            // Gửi thông báo đăng ký thành công qua email
            var registrationEmailSubject = "Welcome to Growth Tracking System!";
            var registrationEmailBody = "Your account has been successfully created. Please verify your email to activate your account.";
            await _emailService.SendEmailAsync(user.Email, registrationEmailSubject, registrationEmailBody);

            // Gửi OTP qua SMS nếu có số điện thoại
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                var sendOtpDTO = new SendOtpDTO(user.PhoneNumber);
                await _userRepository.SendOtp(sendOtpDTO);
            }
        }

        public async Task SendForgotPasswordEmailAsync(string email, string otpCode)
        {
            var subject = "Your Password Reset Code";
            var body = $"Your password reset code is: <strong>{otpCode}</strong>. This code will expire in 1 hour.";
            await _emailService.SendEmailAsync(email, subject, body);
            _logger.LogInformation($"Sent password reset code to {email}");
        }

        public async Task SendForgotPasswordSmsAsync(string phoneNumber, string otpCode)
        {
            var sendOtpDTO = new SendOtpDTO(phoneNumber);
            await _smsService.SendOtpAsync(sendOtpDTO);
            _logger.LogInformation($"Sent password reset code to {phoneNumber} via SMS");
        }
    }
}