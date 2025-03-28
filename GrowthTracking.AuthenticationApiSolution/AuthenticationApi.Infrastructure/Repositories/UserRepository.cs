using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AuthenticationDbContext _context;
        private readonly IConfiguration _config;
        private readonly INotificationService _notificationService; // Thay NotificationSender bằng INotificationService
        private readonly ISmsService _smsService;

        public UserRepository(AuthenticationDbContext context, IConfiguration config, INotificationService notificationService, ISmsService smsService)
        {
            _context = context;
            _config = config;
            _notificationService = notificationService; // Cập nhật constructor
            _smsService = smsService;
        }

        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == appUserDTO.Email);
            if (existingUser != null)
            {
                return new Response(false, "Email already registered");
            }

            var verificationToken = Guid.NewGuid().ToString();
            var user = new AppUser
            {
                UserAccountID = appUserDTO.UserAccountID ?? Guid.NewGuid(),
                FullName = appUserDTO.FullName,
                Email = appUserDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(appUserDTO.PasswordHash),
                PhoneNumber = appUserDTO.PhoneNumber,
                Role = appUserDTO.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = appUserDTO.IsActive,
                ProfilePictureUrl = appUserDTO.ProfilePictureUrl,
                Address = appUserDTO.Address,
                Bio = appUserDTO.Bio,
                EmailVerified = false,
                PhoneVerified = false,
                VerificationToken = verificationToken,
                ResetToken = appUserDTO.ResetToken,
                ResetTokenExpiry = appUserDTO.ResetTokenExpiry,
                OAuth2GoogleId = appUserDTO.OAuth2GoogleId,
                OAuth2FacebookId = appUserDTO.OAuth2FacebookId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Gửi thông báo đăng ký
            await _notificationService.SendRegistrationNotificationAsync(user.Adapt<AppUserDTO>()); // Sử dụng INotificationService

            return new Response(true, "User registered successfully. Please verify your email and phone number.");
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new Response(false, "Invalid credentials");
            }

            if (!user.IsActive.GetValueOrDefault())
            {
                return new Response(false, "Account is inactive. Please verify your email.");
            }

            if (!user.EmailVerified.GetValueOrDefault())
            {
                return new Response(false, "Please verify your email before logging in.");
            }

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "Login successful");
        }

        public async Task<AppUserDTO?> GetUser(Guid userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserAccountID == userId);
            return user?.Adapt<AppUserDTO>();
        }

        public async Task<Response> CreateBugReport(BugReportDTO bugReportDTO)
        {
            var bugReport = bugReportDTO.Adapt<BugReport>();
            bugReport.CreatedAt = DateTime.UtcNow;
            bugReport.UpdatedAt = DateTime.UtcNow;
            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();
            return new Response(true, "Bug report created successfully");
        }

        public async Task<IEnumerable<BugReportDTO>> GetBugReports(Guid userId)
        {
            var bugReports = await _context.BugReports
                .Where(br => br.UserId == userId)
                .ToListAsync();
            return bugReports.Adapt<IEnumerable<BugReportDTO>>();
        }

        public async Task<Response> SendNotification(NotificationDTO notificationDTO)
        {
            var notification = notificationDTO.Adapt<Notification>();
            notification.CreatedAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return new Response(true, "Notification sent successfully");
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotifications(Guid userId)
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();
            return notifications.Adapt<IEnumerable<NotificationDTO>>();
        }

        public async Task<Response> UpdateUser(Guid userId, string fullName)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserAccountID == userId);
            if (user == null)
                return new Response(false, "User not found");

            user.FullName = fullName;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return new Response(true, "User updated successfully");
        }

        public async Task<Response> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            // Validate DTO
            forgotPasswordDTO.Validate();

            AppUser? user = null;
            if (!string.IsNullOrEmpty(forgotPasswordDTO.Email))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDTO.Email);
                if (user == null)
                    return new Response(false, "Email not found");
            }
            else if (!string.IsNullOrEmpty(forgotPasswordDTO.PhoneNumber))
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == forgotPasswordDTO.PhoneNumber);
                if (user == null)
                    return new Response(false, "Phone number not found");
            }

            // Tạo mã OTP 6 ký tự (chỉ chứa số)
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString(); // Mã 6 chữ số
            user!.ResetToken = otpCode;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Mã hết hạn sau 1 giờ
            await _context.SaveChangesAsync();

            // Gửi mã OTP qua email hoặc SMS
            try
            {
                if (!string.IsNullOrEmpty(forgotPasswordDTO.Email))
                {
                    await _notificationService.SendForgotPasswordEmailAsync(user.Email!, otpCode); // Sử dụng INotificationService
                    return new Response(true, "Password reset code has been sent to your email.");
                }
                else
                {
                    await _notificationService.SendForgotPasswordSmsAsync(user.PhoneNumber!, otpCode); // Sử dụng INotificationService
                    return new Response(true, "Password reset code has been sent to your phone number.");
                }
            }
            catch (Exception ex)
            {
                return new Response(false, $"Failed to send reset code: {ex.Message}");
            }
        }

        public async Task<Response> ResetPassword(ResetPasswordDTO resetPasswordDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetPasswordDTO.Code);
            if (user == null)
                return new Response(false, "Invalid code");

            if (user.ResetTokenExpiry < DateTime.UtcNow)
                return new Response(false, "Code has expired");

            // Xác thực mật khẩu cũ
            if (string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(resetPasswordDTO.OldPassword, user.PasswordHash))
                return new Response(false, "Incorrect old password");

            // Cập nhật mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(resetPasswordDTO.NewPassword);
            user.ResetToken = null;
            user.ResetTokenExpiry = null;
            await _context.SaveChangesAsync();

            return new Response(true, "Password reset successfully");
        }

        public async Task<Response> VerifyEmail(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
                return new Response(false, "Invalid verification token");

            user.EmailVerified = true;
            user.VerificationToken = null;
            await _context.SaveChangesAsync();

            return new Response(true, "Email verified successfully");
        }

        public async Task<Response> SendOtp(SendOtpDTO sendOtpDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == sendOtpDTO.PhoneNumber);
            if (user == null)
                return new Response(false, "Phone number not found");

            if (user.PhoneVerified.GetValueOrDefault())
                return new Response(false, "Phone number is already verified");

            try
            {
                await _smsService.SendOtpAsync(sendOtpDTO);
                return new Response(true, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, $"Failed to send OTP: {ex.Message}");
            }
        }

        public async Task<Response> VerifyPhoneNumber(VerifyPhoneDTO verifyPhoneDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == verifyPhoneDTO.PhoneNumber);
            if (user == null)
                return new Response(false, "Phone number not found");

            if (user.PhoneVerified.GetValueOrDefault())
                return new Response(false, "Phone number is already verified");

            try
            {
                var isValidOtp = await _smsService.VerifyOtpAsync(verifyPhoneDTO);
                if (!isValidOtp)
                    return new Response(false, "Invalid or expired OTP");

                user.PhoneVerified = true;
                await _context.SaveChangesAsync();

                return new Response(true, "Phone number verified successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, $"Failed to verify OTP: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AppUserDTO>> GetUnverifiedUsersAsync()
        {
            var users = await _context.Users
                .Where(u => !u.EmailVerified.GetValueOrDefault())
                .ToListAsync();
            return users.Adapt<IEnumerable<AppUserDTO>>();
        }
    }
}