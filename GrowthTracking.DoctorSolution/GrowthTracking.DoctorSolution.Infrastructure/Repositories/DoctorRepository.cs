using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;

namespace GrowthTracking.DoctorSolution.Infrastructure.Repositories
{
    public class DoctorRepository(SWD_GrowthTrackingSystemDbContext context) : GenericRepository<Doctor>(context), IDoctorRepository
    {
    }
}
