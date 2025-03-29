using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Domain.Entities;
using GrowthTracking.ShareLibrary.Response;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<Response> Register(AppUserDTO appUserDTO);
        Task<Response> Login(LoginDTO loginDTO);
        Task<AppUserDTO?> GetUser(Guid userId);
        Task<Response> CreateBugReport(BugReportDTO bugReportDTO);
        Task<IEnumerable<BugReportDTO>> GetBugReports(Guid userId);
        Task<Response> SendNotification(NotificationDTO notificationDTO);
        Task<IEnumerable<NotificationDTO>> GetNotifications(Guid userId);
        Task<Response> UpdateUser(Guid userId, string fullName);
        Task<Response> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task<Response> ResetPassword(ResetPasswordDTO resetPasswordDTO);
        Task<Response> VerifyEmail(string token);
        Task<Response> SendOtp(SendOtpDTO sendOtpDTO);
        Task<Response> VerifyPhoneNumber(VerifyPhoneDTO verifyPhoneDTO);
        Task<IEnumerable<AppUser>> GetUnverifiedUsersAsync();
    }
}