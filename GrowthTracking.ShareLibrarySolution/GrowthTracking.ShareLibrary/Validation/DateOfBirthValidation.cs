using System.ComponentModel.DataAnnotations;

namespace GrowthTracking.ShareLibrary.Validation
{
    public class DateOfBirthValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Allow null if DateOfBirth is not required (or handle null appropriately)
            if (value == null)
                return ValidationResult.Success;

            // Try to parse the input value into a DateTime.
            if (!DateTime.TryParse(value.ToString(), out DateTime dateOfBirth))
                return new ValidationResult("Invalid date format.");

            // Check that the date of birth is before today's date.
            if (dateOfBirth >= DateTime.Today)
                return new ValidationResult("Date of birth must be in the past.");

            // Calculate age.
            int age = DateTime.Today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            // Verify that the doctor is at least 18 years old.
            if (age < 18)
                return new ValidationResult("Doctor must be at least 18 years old.");

            return ValidationResult.Success;
        }
    }
}
