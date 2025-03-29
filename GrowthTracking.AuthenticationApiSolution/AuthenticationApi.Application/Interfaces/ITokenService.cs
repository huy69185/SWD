using AuthenticationApi.Domain.Entities;

namespace AuthenticationApi.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(AppUser user);
    }
}
