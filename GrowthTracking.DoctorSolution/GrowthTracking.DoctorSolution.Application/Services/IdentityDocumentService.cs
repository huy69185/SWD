using GrowthTracking.DoctorSolution.Application.Services.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Domain.Enums;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class IdentityDocumentService : IIdentityDocumentService
    {
        public Task UpdateDoctorStatusIfVerified(Guid doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityDocument> UpdateDocumentStatusAsync(Guid documentId, DocumentStatus newStatus, Guid adminId)
        {
            throw new NotImplementedException();
        }
    }
}
