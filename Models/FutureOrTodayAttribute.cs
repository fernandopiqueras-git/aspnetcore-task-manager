using System.ComponentModel.DataAnnotations;

namespace AspNetCoreTaskManager.Models;

public class FutureOrTodayAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        return value is DateTime date && date.Date >= DateTime.Today
            ? ValidationResult.Success
            : new ValidationResult("La fecha límite no puede estar en el pasado.");
    }
}
