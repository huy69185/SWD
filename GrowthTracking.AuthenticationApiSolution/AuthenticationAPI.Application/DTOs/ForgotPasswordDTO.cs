using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.DTOs
{
    public record ForgotPasswordDTO
    {
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; init; }

        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? PhoneNumber { get; init; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(PhoneNumber))
            {
                throw new ValidationException("Either Email or PhoneNumber must be provided.");
            }
        }
    }
}