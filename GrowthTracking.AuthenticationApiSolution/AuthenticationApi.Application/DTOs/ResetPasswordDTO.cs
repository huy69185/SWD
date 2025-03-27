using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record ResetPasswordDTO(
        [Required] string Code, 
        [Required] string OldPassword,
        [Required, MinLength(8)] string NewPassword 
    );
}