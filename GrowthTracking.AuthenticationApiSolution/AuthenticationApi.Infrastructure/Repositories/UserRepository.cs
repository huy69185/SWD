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
    public class UserRepository(AuthenticationDbContext context,
        IConfiguration config,
        INotificationService notificationService,
        ISmsService smsService,
        ITokenService tokenService) : IUserRepository
    {
        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == appUserDTO.Email);
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
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                PhoneNumber = appUserDTO.PhoneNumber,
                Role = appUserDTO.Role,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsActive = true,
                ProfilePictureUrl = appUserDTO.ProfilePictureUrl,
                Address = appUserDTO.Address,
                Bio = appUserDTO.Bio,
                EmailVerified = false,
                PhoneVerified = false,
                VerificationToken = verificationToken
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            // Gửi thông báo đăng ký
            await notificationService.SendRegistrationNotificationAsync(user.Email, verificationToken); // Sử dụng INotificationService

            return new Response(true, "User registered successfully. Please verify your email and phone number.");
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
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
            await context.SaveChangesAsync();
            string token = tokenService.GenerateToken(user);
            return new Response(true, token);
        }

        public async Task<AppUserDTO?> GetUser(Guid userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserAccountID == userId);
            return user?.Adapt<AppUserDTO>();
        }

        public async Task<Response> CreateBugReport(BugReportDTO bugReportDTO)
        {
            var bugReport = bugReportDTO.Adapt<BugReport>();
            bugReport.CreatedAt = DateTime.UtcNow;
            bugReport.UpdatedAt = DateTime.UtcNow;
            context.BugReports.Add(bugReport);
            await context.SaveChangesAsync();
            return new Response(true, "Bug report created successfully");
        }

        public async Task<IEnumerable<BugReportDTO>> GetBugReports(Guid userId)
        {
            var bugReports = await context.BugReports
                .Where(br => br.UserId == userId)
                .ToListAsync();
            return bugReports.Adapt<IEnumerable<BugReportDTO>>();
        }

        public async Task<Response> SendNotification(NotificationDTO notificationDTO)
        {
            var notification = notificationDTO.Adapt<Notification>();
            notification.CreatedAt = DateTime.UtcNow;
            notification.UpdatedAt = DateTime.UtcNow;
            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
            return new Response(true, "Notification sent successfully");
        }

        public async Task<IEnumerable<NotificationDTO>> GetNotifications(Guid userId)
        {
            var notifications = await context.Notifications
                .Where(n => n.UserId == userId)
                .ToListAsync();
            return notifications.Adapt<IEnumerable<NotificationDTO>>();
        }

        public async Task<Response> UpdateUser(Guid userId, string fullName)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserAccountID == userId);
            if (user == null)
                return new Response(false, "User not found");

            user.FullName = fullName;
            user.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return new Response(true, "User updated successfully");
        }

        public async Task<Response> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO)
        {
            // Validate DTO
            forgotPasswordDTO.Validate();

            AppUser? user = null;
            if (!string.IsNullOrEmpty(forgotPasswordDTO.Email))
            {
                user = await context.Users.FirstOrDefaultAsync(u => u.Email == forgotPasswordDTO.Email);
                if (user == null)
                    return new Response(false, "Email not found");
            }
            else if (!string.IsNullOrEmpty(forgotPasswordDTO.PhoneNumber))
            {
                user = await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == forgotPasswordDTO.PhoneNumber);
                if (user == null)
                    return new Response(false, "Phone number not found");
            }

            // Tạo mã OTP 6 ký tự (chỉ chứa số)
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString(); // Mã 6 chữ số
            user!.ResetToken = otpCode;
            user.ResetTokenExpiry = DateTime.UtcNow.AddHours(1); // Mã hết hạn sau 1 giờ
            await context.SaveChangesAsync();

            // Gửi mã OTP qua email hoặc SMS
            try
            {
                if (!string.IsNullOrEmpty(forgotPasswordDTO.Email))
                {
                    await notificationService.SendForgotPasswordEmailAsync(user.Email!, otpCode); // Sử dụng INotificationService
                    return new Response(true, "Password reset code has been sent to your email.");
                }
                else
                {
                    await notificationService.SendForgotPasswordSmsAsync(user.PhoneNumber!, otpCode); // Sử dụng INotificationService
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
            var user = await context.Users.FirstOrDefaultAsync(u => u.ResetToken == resetPasswordDTO.Code);
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
            await context.SaveChangesAsync();

            return new Response(true, "Password reset successfully");
        }

        public async Task<Response> VerifyEmail(string token)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user == null)
                return new Response(false, "Invalid verification token");

            user.EmailVerified = true;
            user.VerificationToken = null;
            await context.SaveChangesAsync();

            return new Response(true, "Email verified successfully");
        }

        public async Task<Response> SendOtp(SendOtpDTO sendOtpDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == sendOtpDTO.PhoneNumber);
            if (user == null)
                return new Response(false, "Phone number not found");

            if (user.PhoneVerified.GetValueOrDefault())
                return new Response(false, "Phone number is already verified");

            try
            {
                await smsService.SendOtpAsync(sendOtpDTO);
                return new Response(true, "OTP sent successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, $"Failed to send OTP: {ex.Message}");
            }
        }

        public async Task<Response> VerifyPhoneNumber(VerifyPhoneDTO verifyPhoneDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.PhoneNumber == verifyPhoneDTO.PhoneNumber);
            if (user == null)
                return new Response(false, "Phone number not found");

            if (user.PhoneVerified.GetValueOrDefault())
                return new Response(false, "Phone number is already verified");

            try
            {
                var isValidOtp = await smsService.VerifyOtpAsync(verifyPhoneDTO);
                if (!isValidOtp)
                    return new Response(false, "Invalid or expired OTP");

                user.PhoneVerified = true;
                await context.SaveChangesAsync();

                return new Response(true, "Phone number verified successfully");
            }
            catch (Exception ex)
            {
                return new Response(false, $"Failed to verify OTP: {ex.Message}");
            }
        }

        public async Task<IEnumerable<AppUser>> GetUnverifiedUsersAsync()
        {
            return await context.Users
                .Where(u => !u.EmailVerified.GetValueOrDefault())
                .ToListAsync();
        }
    }
}