using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendRegistrationNotificationAsync(AppUserDTO user);
        Task SendForgotPasswordEmailAsync(string email, string otpCode);
        Task SendForgotPasswordSmsAsync(string phoneNumber, string otpCode);
    }
}