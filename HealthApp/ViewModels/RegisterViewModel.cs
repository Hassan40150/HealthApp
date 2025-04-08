using System.ComponentModel.DataAnnotations;


namespace HealthApp.ViewModels
{
    public class RegisterViewModel
    {

        public required string Name { get; set; }

        public required string Password { get; set; }

        [EmailAddress]
        public required string Email { get; set; }


    }
}
