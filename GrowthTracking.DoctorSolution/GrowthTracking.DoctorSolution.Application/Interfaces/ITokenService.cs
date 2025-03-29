using GrowthTracking.DoctorSolution.Infrastructure.DBContext;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(UserAccount user);
    }
}
