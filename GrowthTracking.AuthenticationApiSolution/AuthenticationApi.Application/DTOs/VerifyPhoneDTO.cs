using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record VerifyPhoneDTO(
        [Required, Phone(ErrorMessage = "Invalid phone number format")] string PhoneNumber,
        [Required] string Code
    );
}