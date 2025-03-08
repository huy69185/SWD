using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using GrowthTracking.ShareLibrary.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUserRepository
    {
        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == appUserDTO.Email);
            if (existingUser != null)
            {
                return new Response(false, "Email already registered");
            }

            var user = new AppUser
            {
                UserAccountID = Guid.NewGuid(),
                FullName = appUserDTO.FullName,
                Email = appUserDTO.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(appUserDTO.PasswordHash),
                PhoneNumber = appUserDTO.PhoneNumber,
                Role = appUserDTO.Role,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = appUserDTO.IsActive,
                ProfilePictureUrl = appUserDTO.ProfilePictureUrl,
                Address = appUserDTO.Address,
                Bio = appUserDTO.Bio,
                EmailVerified = appUserDTO.EmailVerified,
                VerificationToken = appUserDTO.VerificationToken,
                ResetToken = appUserDTO.ResetToken,
                ResetTokenExpiry = appUserDTO.ResetTokenExpiry,
                OAuth2GoogleId = appUserDTO.OAuth2GoogleId,
                OAuth2FacebookId = appUserDTO.OAuth2FacebookId
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();
            return new Response(true, "User registered successfully");
        }

        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (user == null || string.IsNullOrEmpty(user.PasswordHash) || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.PasswordHash))
            {
                return new Response(false, "Invalid credentials");
            }

            user.LastLoginAt = DateTime.Now;
            await context.SaveChangesAsync();
            return new Response(true, "Login successful");
        }

        public async Task<AppUserDTO?> GetUser(Guid userId)
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.UserAccountID == userId);
            return user?.Adapt<AppUserDTO>();
        }

        public async Task<Response> CreateBugReport(BugReportDTO bugReportDTO)
        {
            var bugReport = bugReportDTO.Adapt<BugReport>();
            bugReport.CreatedAt = DateTime.Now;
            bugReport.UpdatedAt = DateTime.Now;
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
            notification.CreatedAt = DateTime.Now;
            notification.UpdatedAt = DateTime.Now;
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
    }
}