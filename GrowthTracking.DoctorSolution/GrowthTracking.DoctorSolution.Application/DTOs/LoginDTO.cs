using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.DoctorSolution.Application.DTOs
{
    public record LoginDTO(
        [Required, EmailAddress] string Email,
        [Required] string Password);
}
