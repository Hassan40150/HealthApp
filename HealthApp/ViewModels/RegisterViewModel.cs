using System.ComponentModel.DataAnnotations;
using HealthApp.Attributes;


namespace HealthApp.ViewModels
{
    public class RegisterViewModel
    {

        public required string Name { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$",
        ErrorMessage = @"<div class='text-danger register-password'>
                        Password requirements:
                        <ul>
                            <li>Minimum of 8 characters</li>
                            <li>At least one uppercase letter</li>
                            <li>At least one number</li>
                        </ul>
                     </div>")]
        public required string Password { get; set; }

        [EmailAddress]
        [UniqueEmail]
        public required string Email { get; set; }


    }
}
