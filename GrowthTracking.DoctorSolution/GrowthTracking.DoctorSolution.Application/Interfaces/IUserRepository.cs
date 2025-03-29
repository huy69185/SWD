using GrowthTracking.DoctorSolution.Infrastructure.DBContext;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface IUserRepository : IGenericRepository<UserAccount>
    {
        Task<UserAccount?> GetUserByEmail(string email);
    }
}
