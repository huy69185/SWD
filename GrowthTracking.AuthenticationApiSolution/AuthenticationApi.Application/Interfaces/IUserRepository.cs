using AuthenticationApi.Application.DTOs;
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
    }
}