using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record LoginDTO(
        [Required, EmailAddress] string Email,
        [Required] string Password
    );
}