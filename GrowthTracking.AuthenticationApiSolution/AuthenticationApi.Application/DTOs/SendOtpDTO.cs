using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record SendOtpDTO(
        [Required, Phone(ErrorMessage = "Invalid phone number format")] string PhoneNumber
    );
}