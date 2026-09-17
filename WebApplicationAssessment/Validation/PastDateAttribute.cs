using System.ComponentModel.DataAnnotations;

namespace WebApplicationAssessment.Validation
{
    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.Date >= DateTime.Today)
                {
                    return new ValidationResult(ErrorMessage ?? "Date of birth must be a past date.");
                }
            }
            return ValidationResult.Success;
        }
    }
}
