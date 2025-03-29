using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace GrowthTracking.DoctorSolution.Infrastructure.Repositories
{
    public class UserRepository(SWD_GrowthTrackingSystemDbContext context) : GenericRepository<UserAccount>(context), IUserRepository
    {
        public async Task<UserAccount?> GetUserByEmail(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }
    }
}
