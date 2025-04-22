using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingGenderViewModel
    {
        [Required]
        [RegularExpression("^(male|female)$", ErrorMessage = "Invalid selection")]
        public string Sex { get; set; } = string.Empty;
    }
}
