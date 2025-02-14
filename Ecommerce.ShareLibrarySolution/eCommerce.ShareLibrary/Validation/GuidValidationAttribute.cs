using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eCommerce.ShareLibrary.Validation
{
    public class GuidValidationAttribute : ValidationAttribute
    {
        public GuidValidationAttribute()
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
