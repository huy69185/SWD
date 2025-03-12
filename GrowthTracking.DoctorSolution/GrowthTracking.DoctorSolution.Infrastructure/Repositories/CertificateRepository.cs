using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Infrastructure.DBContext;

namespace GrowthTracking.DoctorSolution.Infrastructure.Repositories
{
    public class CertificateRepository(SWD_GrowthTrackingSystemDbContext dbContext) : 
        GenericRepository<Certificate>(dbContext), ICertificateRepository
    {
    }
}
