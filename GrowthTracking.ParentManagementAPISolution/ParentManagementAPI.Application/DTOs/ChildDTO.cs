using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace ParentManageApi.Application.DTOs
{
    public record ChildDTO(
        [GuidValidation] Guid ChildId,
        [GuidValidation] Guid ParentId,
        [Required] string FullName,
        [Required] DateTime DateOfBirth,
        string? Gender,
        decimal? BirthWeight,
        decimal? BirthHeight,
        string? AvatarUrl,
        DateTime? CreatedAt,
        DateTime? UpdatedAt
    );
}