using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Domain.Enums;

namespace GrowthTracking.DoctorSolution.Application.Services.Interfaces
{
    public interface IIdentityDocumentService
    {
        Task<IdentityDocument> UpdateDocumentStatusAsync(Guid documentId, DocumentStatus newStatus, Guid adminId);
        Task UpdateDoctorStatusIfVerified(Guid doctorId);
    }
}
