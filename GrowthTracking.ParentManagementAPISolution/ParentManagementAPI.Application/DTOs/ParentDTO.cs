using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace ParentManageApi.Application.DTOs
{
    public record ParentDTO(
        [Required(ErrorMessage = "Full name is required")]
        string FullName,

        [DataType(DataType.Date)]
        DateTime? DateOfBirth,

        [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
        string? Gender,

        string? Address,

        [Url(ErrorMessage = "Avatar URL must be a valid URL")]
        string? AvatarUrl
    );
}