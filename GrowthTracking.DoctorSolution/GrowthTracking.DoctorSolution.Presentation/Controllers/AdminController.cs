using GrowthTracking.DoctorSolution.Application.DTOs;
using GrowthTracking.DoctorSolution.Application.Interfaces;
using GrowthTracking.DoctorSolution.Domain.Constants;
using GrowthTracking.DoctorSolution.Domain.Enums;
using GrowthTracking.ShareLibrary.Response;
using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GrowthTracking.DoctorSolution.Presentation.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = Consts.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IIdentityDocumentService _documentService;

        public AdminController(IDoctorService doctorService, IIdentityDocumentService documentService)
        {
            _doctorService = doctorService;
            _documentService = documentService;
        }

        [HttpGet("doctors/pending")]
        [EndpointSummary("Get all doctors pending verification")]
        public async Task<IActionResult> GetPendingDoctors(int page = 1, int pageSize = 10)
        {
            var result = await _doctorService.GetAllDoctors(page, pageSize);
            var pendingDoctors = result.Where(d => d.Status == DoctorStatus.PendingVerification.ToString()).ToList();

            return Ok(new ApiResponse
            {
                Success = true,
                Data = new
                {
                    TotalCount = pendingDoctors.Count,
                    doctorList = pendingDoctors
                }
            });
        }

        [HttpPost("doctors/{doctorId}/verify")]
        [EndpointSummary("Verify a doctor's credentials")]
        public async Task<IActionResult> VerifyDoctor([GuidValidation] string doctorId, [FromBody] DoctorVerificationRequest request)
        {
            string adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            // Update doctor status based on approval
            var newStatus = request.IsApproved ? DoctorStatus.Active.ToString() : DoctorStatus.Inactive.ToString();
            var result = await _doctorService.UpdateDoctorStatus(doctorId, newStatus);

            // If we have additional document verification logic, we could add it here

            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Doctor verification status updated to {newStatus}",
                Data = result
            });
        }

        [HttpGet("documents/pending")]
        [EndpointSummary("Get all pending identity documents")]
        public async Task<IActionResult> GetPendingDocuments(int page = 1, int pageSize = 10)
        {
            var result = await _documentService.GetPendingDocumentsAsync(page, pageSize);

            return Ok(new ApiResponse
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

        [HttpPost("documents/{documentId}/verify")]
        [EndpointSummary("Verify an identity document")]
        public async Task<IActionResult> VerifyDocument([GuidValidation] string documentId, [FromBody] DocumentVerificationRequest request)
        {
            string adminId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var docStatus = request.IsApproved ? DocumentStatus.Approved.ToString() : DocumentStatus.Rejected.ToString();
            var document = await _documentService.UpdateDocumentStatusAsync(documentId, docStatus, adminId);

            // Check if all documents are verified for this doctor
            if (request.IsApproved)
            {
                await _documentService.UpdateDoctorStatusIfVerified(document.DoctorId.ToString());
            }

            return Ok(new ApiResponse
            {
                Success = true,
                Message = $"Document verification status updated to {docStatus}",
                Data = document
            });
        }
    }
}
