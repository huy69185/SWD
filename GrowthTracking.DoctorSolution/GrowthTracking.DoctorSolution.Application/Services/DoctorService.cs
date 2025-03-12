using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Application.Mapping;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.ShareLibrary.Pagination;
using System.Linq.Expressions;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class DoctorService(IDoctorRepository repo, IUserService userService, IMapperService mapper) : IDoctorService
    {
        public Task<DoctorResponse> CreateDoctor(DoctorCreateRequest doctor)
        {
            throw new NotImplementedException();
        }

        public Task DeleteDoctor(string doctorId)
        {
            throw new NotImplementedException();
        }

        public async Task<PagedList<DoctorResponse>> GetAllDoctors(int page, int pageSize)
        {
            var doctors = await repo.GetPagedAsync(
                page, pageSize, 
                orderBy: list => list.OrderByDescending(d => d.CreatedAt));
            return doctors.MapPagedList<Doctor, DoctorResponse>(mapper);
        }

        public async Task<DoctorResponse?> GetDoctorById(string doctorId)
        {
            var doctor = await repo.GetByIdAsync(doctorId);
            return doctor == null ? null : mapper.Map<Doctor, DoctorResponse>(doctor);
        }

        public async Task<PagedList<DoctorResponse>> SearchDoctors(string searchTerm, int page, int pageSize)
        {
            searchTerm = searchTerm.ToLowerInvariant();
            Expression<Func<Doctor, bool>> searchExpression = d =>
                d.FullName.Contains(searchTerm) ||
                (d.Workplace != null && d.Workplace.Contains(searchTerm)) ||
                (d.Biography != null && d.Biography.Contains(searchTerm));

            var doctors = await repo.GetPagedAsync(page, pageSize, 
                filter: searchExpression,
                orderBy: list => list.OrderByDescending(d => d.CreatedAt));

            return doctors.MapPagedList<Doctor, DoctorResponse>(mapper);
        }

        public Task<DoctorResponse> UpdateDoctor(DoctorUpdateRequest doctor)
        {
            throw new NotImplementedException();
        }

        public Task<DoctorResponse> UpdateDoctorStatus(string doctorId, string newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
