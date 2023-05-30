using System.ComponentModel.DataAnnotations;

namespace BotterApi.Validations;

public class BirthDayValidationAttribute: ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(Convert.ToDateTime(value).Year > 2010)
        {
            return new ValidationResult("User must be over 13 years old to register");
        }
        return ValidationResult.Success;
    }
}
