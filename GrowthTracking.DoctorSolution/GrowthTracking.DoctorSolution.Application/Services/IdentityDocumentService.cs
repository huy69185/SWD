using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Entities;
using GrowthTracking.DoctorSolution.Domain.Enums;
using GrowthTracking.ShareLibrary.Pagination;

namespace GrowthTracking.DoctorSolution.Application.Services
{
    public class IdentityDocumentService(IIdentityDocumentRepository repo) : IIdentityDocumentService
    {
        public async Task<PagedList<IdentityDocument>> GetPendingDocumentsAsync(int page, int pageSize)
        {
            var pendingStatus = DocumentStatus.Pending.ToString();

            var documents = await repo.GetPagedAsync(page, pageSize,
                filter: d => d.Status == pendingStatus,
                orderBy: list => list.OrderByDescending(d => d.CreatedAt));

            return documents;
        }

        public Task UpdateDoctorStatusIfVerified(string doctorId)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityDocument> UpdateDocumentStatusAsync(string documentId, string newStatus, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<IdentityDocument> UpdateDocumentStatusAsync(string documentId, string newStatus, string adminId)
        {
            var document = await repo.GetByIdAsync(Guid.Parse(documentId))
                ?? throw new NotFoundException($"Document with ID {documentId} not found.");

            document.Status = newStatus;
            document.UpdatedAt = DateTime.UtcNow;
            document.VerifiedByAdminId = Guid.Parse(adminId);

            await repo.UpdateAsync(document);
            await repo.SaveAsync();

            return document;
        }

        public async Task UpdateDoctorStatusIfVerified(string doctorId)
        {
            var documents = await repo.GetAllAsync(d => d.DoctorId == Guid.Parse(doctorId));

            // Check if all documents are approved
            bool allApproved = documents.All(d => d.Status == DocumentStatus.Approved.ToString());

            if (allApproved)
            {
                // Update doctor status through IDoctorRepository
                var doctor = await _doctorRepository.GetByIdAsync(Guid.Parse(doctorId))
                    ?? throw new NotFoundException($"Doctor with ID {doctorId} not found.");

                doctor.Status = DoctorStatus.Active.ToString();
                doctor.UpdatedAt = DateTime.UtcNow;

                await _doctorRepository.UpdateAsync(doctor);
                await _doctorRepository.SaveAsync();
            }
        }

    }
}
