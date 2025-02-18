using System.Threading.Tasks;
using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse> AuthenticateWithOAuth(OAuthLoginDTO loginDTO);
    }
}
