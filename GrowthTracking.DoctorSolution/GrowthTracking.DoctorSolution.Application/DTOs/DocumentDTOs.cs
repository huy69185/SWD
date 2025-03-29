using GrowthTracking.ShareLibrary.Validation;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.DTOs
{
    public record DocumentCreateRequest(
        [GuidValidation] string DoctorId,
        [Required] string CertificateType,
        string? CertificateNumber,
        [Required] string IssuingAuthority,
        [DateOnlyValidation] string? IssueDate,
        [DateOnlyValidation] string? ExpiryDate,
        IFormFile? DocumentFile
    );

    public class DoctorVerificationRequest
    {
        public bool IsApproved { get; set; }
        public string? Remarks { get; set; }
    }

    public class DocumentVerificationRequest
    {
        public bool IsApproved { get; set; }
        public string? Remarks { get; set; }
    }

}
