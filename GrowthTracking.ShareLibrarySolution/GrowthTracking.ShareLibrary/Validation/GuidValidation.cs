using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.ShareLibrary.Validation
{
    public class GuidValidation : ValidationAttribute
    {
        public GuidValidation()
        {
            ErrorMessage = "Invalid Guid attribute";  // Default error message
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!; // If the value is null, we consider it valid (optional)
            }

            var valueAsString = value as string;

            if (string.IsNullOrEmpty(valueAsString) || !Guid.TryParse(valueAsString, out _))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
