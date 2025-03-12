using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Application.Mapping;
using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.ShareLibrary.Exceptions;
using GrowthTracking.ShareLibrary.Pagination;
using System.Linq.Expressions;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class DoctorService(IDoctorRepository repo, IUserService userService, IMapperService mapper) : IDoctorService
    {
        public async Task<DoctorResponse> CreateDoctor(DoctorCreateRequest doctor)
        {
            // Step 1: Check if User Exists via User Microservice
            var userExists = await userService.CheckUserExists(doctor.DoctorId);
            if (!userExists)
            {
                throw new NotFoundException("User account not found in User Microservice.");
            }

            // Step 2: Ensure User is Not Already a Doctor
            var existingDoctor = await repo.GetByIdAsync(doctor.DoctorId);
            if (existingDoctor != null)
            {
                throw new Exception("This user is already registered as a doctor.");
            }

            // Step 3: Map request to entity
            var entity = mapper.Map<DoctorCreateRequest, Doctor>(doctor);
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            // Step 4: Save to Database
            await repo.InsertAsync(entity);
            await repo.SaveAsync();

            // Step 5: Map and return response
            return mapper.Map<Doctor, DoctorResponse>(entity);

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

        public async Task<DoctorResponse> UpdateDoctor(string id, DoctorUpdateRequest doctor)
        {
            // 1. Retrieve the existing doctor
            var entity = await repo.GetByIdAsync(Guid.Parse(id)) 
                ?? throw new NotFoundException($"Doctor with ID {id} not found.");

            // 2. Update fields only if they are provided (not null)
            if (!string.IsNullOrWhiteSpace(doctor.FullName))
                entity.FullName = doctor.FullName;

            if (!string.IsNullOrWhiteSpace(doctor.DateOfBirth))
                entity.DateOfBirth = DateOnly.Parse(doctor.DateOfBirth); // Convert string to DateOnly

            if (!string.IsNullOrWhiteSpace(doctor.Gender))
                entity.Gender = doctor.Gender;

            if (!string.IsNullOrWhiteSpace(doctor.Address))
                entity.Address = doctor.Address;

            if (!string.IsNullOrWhiteSpace(doctor.PhoneNumber))
                entity.PhoneNumber = doctor.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(doctor.Specialization))
                entity.Specialization = doctor.Specialization;

            if (doctor.ExperienceYears.HasValue)
                entity.ExperienceYears = doctor.ExperienceYears.Value;

            if (!string.IsNullOrWhiteSpace(doctor.Workplace))
                entity.Workplace = doctor.Workplace;

            if (!string.IsNullOrWhiteSpace(doctor.Biography))
                entity.Biography = doctor.Biography;

            if (!string.IsNullOrWhiteSpace(doctor.ProfilePhoto))
                entity.ProfilePhoto = doctor.ProfilePhoto;

            // 3. Update timestamp
            entity.UpdatedAt = DateTime.UtcNow;

            // 4. Save changes
            await repo.UpdateAsync(entity);
            await repo.SaveAsync();

            // 5. Return the updated doctor response
            return mapper.Map<Doctor, DoctorResponse>(entity);
        }

        public Task<DoctorResponse> UpdateDoctorStatus(string doctorId, string newStatus)
        {
            throw new NotImplementedException();
        }
    }
}
