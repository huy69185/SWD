using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record BugReportDTO(
        [GuidValidation] Guid BugReportId,
        [Required] Guid UserId,
        string? ReportType,
        [Required] string Description,
        string? ScreenshotUrl,
        [Required] string Status,
        DateTime? CreatedAt = null,
        DateTime? UpdatedAt = null
    );
}