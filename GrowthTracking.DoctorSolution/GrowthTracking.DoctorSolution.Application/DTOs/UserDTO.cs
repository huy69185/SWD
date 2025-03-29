using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.DTOs
{
    public record UserDTO(
        [Required] string FullName,
        [Required, EmailAddress] string Email,
        [Required] string Password,
        string? PhoneNumber,
        string? Role,
        string? ProfilePictureUrl,
        string? Address,
        string? Bio
        );
}
