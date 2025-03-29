using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Domain.Enums;
using GrowthTracking.ShareLibrary.Pagination;

namespace GrowthTracking.DoctorSolution.Application.Interfaces
{
    public interface IIdentityDocumentService
    {
        Task<IdentityDocument> UpdateDocumentStatusAsync(string documentId, string newStatus, string adminId);
        Task UpdateDoctorStatusIfVerified(string doctorId);
        Task<PagedList<IdentityDocument>> GetPendingDocumentsAsync(int page, int pageSize);
    }
}
