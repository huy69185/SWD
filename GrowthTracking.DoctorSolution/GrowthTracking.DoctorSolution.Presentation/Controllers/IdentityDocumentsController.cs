using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Constants;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/identity-documents")]
    [ApiController]
    public class IdentityDocumentsController(IIdentityDocumentService service) : ControllerBase
    {
        [Route("/api/admin/identity-documents/pending")]
        [HttpGet]
        [Authorize(Roles = Consts.Admin)]
        [EndpointDescription("This api is use for admin to retrieve all pending documents")]
        public async Task<IActionResult> GetPendingDocuments(int page = 1, int pageSize = 10)
        {
            var result = await service.GetPendingDocumentsAsync(page, pageSize);
            return Ok(new ApiResponse()
            {
                Success = true,
                Data = new
                {
                    result.TotalCount,
                    result.TotalPages,
                    result.CurrentPage,
                    result.PageSize,
                    result.HasNext,
                    result.HasPrevious,
                    documents = result
                }
            });
        }

        [Route("/api/admin/identity-documents/{documentId}/{status}")]
        [HttpPatch]
        [Authorize(Roles = Consts.Admin)]
        public async Task<IActionResult> UpdateDocumentStatus(string documentId, string status)
        {
            var adminId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
            var document = await service.UpdateDocumentStatusAsync(documentId, status, adminId);
            return Ok(document);
        }

    }
}
