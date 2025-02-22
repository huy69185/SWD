using GrowthTracking.ShareLibrary.Validation;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record AppUserDTO(
        [GuidValidation] string? Id,
        [Required] string Name,
        [Required] string PhoneNumber,
        [Required] string Address,
        [Required, EmailAddress] string Email,
        [Required] string Password,
        [Required] string Role
        );
}
