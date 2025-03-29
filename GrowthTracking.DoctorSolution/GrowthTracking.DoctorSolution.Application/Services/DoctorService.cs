using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Application.Mapping;
using GrowthTracking.DoctorSolution.Application.MessageQueue;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Domain.Enums;
using GrowthTracking.ShareLibrary.Exceptions;
using GrowthTracking.ShareLibrary.Pagination;
using Microsoft.AspNetCore.Http;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class DoctorService(
        IDoctorRepository repo, 
        IUserService userService, 
        IMapperService mapper,
        IFileStorageService storageService,
        IDoctorEventPublisher doctorEventPublisher) : IDoctorService
    {
        public async Task<DoctorResponse> CreateDoctor(DoctorCreateRequest request,
            IFormFile IdCard,
            IFormFile ProfessionalDegree,
            IFormFile MedicalLicense)
        {
            // Step 1: Check if User Exists via User Microservice
            var userExists = await userService.CheckUserExists(request.DoctorId);
            if (!userExists)
            {
                throw new NotFoundException("User account not found in User Microservice.");
            }

            // Step 2: Ensure User is Not Already a Doctor
            var existingDoctor = await repo.GetByIdAsync(request.DoctorId);
            if (existingDoctor != null)
            {
                throw new Exception("This user is already registered as a doctor.");
            }

            // Step 3: Map request to entity
            var entity = mapper.Map<DoctorCreateRequest, Doctor>(request);
            entity.Status = DoctorStatus.PendingVerification.ToString();
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;

            // Upload files and create IdentityDocument records.
            entity.IdentityDocuments.Add(
                await CreateIdentityDocumentAsync(entity.DoctorId, DocumentType.IdCard, request.IdCard));
            entity.IdentityDocuments.Add(
                await CreateIdentityDocumentAsync(entity.DoctorId, DocumentType.ProfessionalDegree, request.ProfessionalDegree));
            entity.IdentityDocuments.Add(
                await CreateIdentityDocumentAsync(entity.DoctorId, DocumentType.MedicalLicense, request.MedicalLicense));


            // Step 4: Save to Database
            await repo.InsertAsync(entity);
            await repo.SaveAsync();

            // Step 5: Publish DoctorCreated event using MassTransit
            // Assuming _doctorEventPublisher is injected and implements IDoctorEventPublisher
            await doctorEventPublisher.PublishDoctorCreatedAsync(entity.DoctorId, entity.FullName);

            // Step 6: Map and return response
            return mapper.Map<Doctor, DoctorResponse>(entity);
        }

        public async Task DeleteDoctor(string doctorId, string currentUserId)
        {
            // 2. Retrieve the doctor
            var entity = await repo.GetByIdAsync(Guid.Parse(doctorId)) 
                ?? throw new NotFoundException($"Doctor with ID {doctorId} not found.");

            // 3. Prevent self-deletion
            if (entity.DoctorId.ToString() == currentUserId)
            {
                throw new ForbiddenException("You cannot delete your own account.");
            }

            // 4. Delete the doctor
            await repo.DeleteAsync(entity);
            await repo.SaveAsync();
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


        public async Task<DoctorResponse> UpdateDoctorStatus(string doctorId, string newStatus)
        {
            var entity = await repo.GetByIdAsync(Guid.Parse(doctorId))
                ?? throw new NotFoundException($"Doctor with ID {doctorId} not found.");

            // Validate status value
            if (!Enum.TryParse<DoctorStatus>(newStatus, out var status))
                throw new ArgumentException($"Invalid status value: {newStatus}");

            entity.Status = status.ToString();
            entity.UpdatedAt = DateTime.UtcNow;

            await repo.UpdateAsync(entity);
            await repo.SaveAsync();

            // Publish status change event
            await doctorEventPublisher.PublishDoctorStatusChangedAsync(entity.DoctorId, entity.Status);

            return mapper.Map<Doctor, DoctorResponse>(entity);
        }

        public Task<DoctorResponse> UpdateDoctorStatus(string doctorId, string newStatus)
        {
            throw new NotImplementedException();
        }

        private async Task<IdentityDocument> CreateIdentityDocumentAsync(Guid doctorId, DocumentType docType, IFormFile file)
        {
            var uploadResult = await storageService.UploadFileAsync(file);
            if (!uploadResult.Success)
                throw new Exception($"File upload failed for {docType}: {uploadResult.ErrorMessage}");

            return new IdentityDocument
            {
                DocumentId = Guid.NewGuid(),
                DoctorId = doctorId,
                Type = docType.ToString(),
                DocumentUrl = uploadResult.Url!,
                Status = DocumentStatus.Pending.ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
        }


    }
}
