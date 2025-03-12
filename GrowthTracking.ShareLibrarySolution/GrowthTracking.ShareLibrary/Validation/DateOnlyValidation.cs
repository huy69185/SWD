using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.ShareLibrary.Validation
{
    public class DateOnlyValidation : ValidationAttribute
    {
        public DateOnlyValidation()
        {
            ErrorMessage = "Invalid DateOnly attribute";  // Default error message
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success!; // If the value is null, we consider it valid (optional)
            }

            var valueAsString = value as string;

            if (string.IsNullOrEmpty(valueAsString) || !DateOnly.TryParse(valueAsString, out _))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success!;
        }
    }
}
