using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace ParentManageApi.Application.DTOs
{
    public record ParentDTO(
        [GuidValidation] Guid ParentId,
        [Required] string FullName,
        DateTime? DateOfBirth,
        string? Gender,
        string? Address,
        string? AvatarUrl,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        bool IsDeleted = false 
    );
}