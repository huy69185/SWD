using AuthenticationApi.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace AuthenticationApi.Application.Validations
{
    public class RoleValidation : ValidationAttribute
    {
        public RoleValidation()
        {
            // Default error message
            ErrorMessage = $"Role must be one of {string.Join(", ", Enum.GetNames<RoleEnum>())}";
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!; // If the value is null, we consider it valid (optional)
            }

            var valueAsString = value as string;

            if (string.IsNullOrEmpty(valueAsString) || !Enum.TryParse<RoleEnum>(valueAsString, true, out _))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
