using AuthenticationApi.Application.Validations;
using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record AppUserDTO(
        [GuidValidation] Guid? UserAccountID,
        [Required] string FullName,
        [Required, EmailAddress] string Email,
        string? Password,
        [Phone] string? PhoneNumber,
        [Required, RoleValidation] string Role,
        string? ProfilePictureUrl = null,
        string? Address = null,
        string? Bio = null
    );
}