using System.ComponentModel.DataAnnotations;
using HealthApp.Attributes;


namespace HealthApp.ViewModels
{
    public class RegisterViewModel
    {

        public required string Name { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$",
    ErrorMessage = "Password must be at least 8 characters long, contain one uppercase letter, and one number.")]

        public required string Password { get; set; }

        [EmailAddress]
        [UniqueEmail]
        public required string Email { get; set; }


    }
}
