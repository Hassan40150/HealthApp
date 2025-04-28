using System.ComponentModel.DataAnnotations;
using HealthApp.Data;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Attributes
{

    public class UniqueEmailAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var _context = validationContext.GetService(typeof(ApplicationDbContext)) as ApplicationDbContext;

            if (_context == null)
            {
                return new ValidationResult("Unable to validate email uniqueness.");
            }

            var email = value?.ToString();

            if (string.IsNullOrEmpty(email))
            {
                return new ValidationResult("Email is required.");
            }

            var exists = _context.Users.Any(u => u.Email == email);

            if (exists)
            {
                return new ValidationResult("This email is already registered.");
            }

            return ValidationResult.Success!;
        }

    }

}
