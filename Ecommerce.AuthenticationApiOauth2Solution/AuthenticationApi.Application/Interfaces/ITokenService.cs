using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Application.DTOs;

namespace AuthenticationApi.Application.Interfaces
{
    public interface ITokenService
    {
        TokenResponseDTO GenerateTokens(AppUser user);
    }
}
