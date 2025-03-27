using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface ISmsService
    {
        Task SendOtpAsync(SendOtpDTO sendOtpDTO);
        Task<bool> VerifyOtpAsync(VerifyPhoneDTO verifyPhoneDTO);
    }
}