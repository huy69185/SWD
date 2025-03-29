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
    }
}
