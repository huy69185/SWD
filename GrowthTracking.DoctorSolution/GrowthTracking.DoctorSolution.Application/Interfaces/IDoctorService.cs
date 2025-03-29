using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.ShareLibrary.Pagination;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface IDoctorService
    {
        Task<DoctorResponse> CreateDoctor(DoctorCreateRequest doctor,
            IFormFile IdCard,
            IFormFile ProfessionalDegree,
            IFormFile MedicalLicense);
        Task<DoctorResponse?> GetDoctorById(string doctorId);
        Task<PagedList<DoctorResponse>> GetAllDoctors(int page, int pageSize);
        Task<DoctorResponse> UpdateDoctor(string id, DoctorUpdateRequest doctor);
        Task DeleteDoctor(string doctorId, string currentUserId);
        Task<DoctorResponse> UpdateDoctorStatus(string doctorId, string newStatus);
        Task<PagedList<DoctorResponse>> SearchDoctors(string searchTerm, int page, int pageSize);

    }
}
