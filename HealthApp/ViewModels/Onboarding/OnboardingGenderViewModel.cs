using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingGenderViewModel
    {
        [Required(ErrorMessage = "Please select your gender.")]
        [RegularExpression("^(male|female)$", ErrorMessage = "Invalid selection.")]
        public string Sex { get; set; } = string.Empty;
    }
}
