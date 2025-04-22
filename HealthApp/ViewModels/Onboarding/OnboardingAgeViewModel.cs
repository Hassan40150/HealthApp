using System.ComponentModel.DataAnnotations;
using HealthApp.Attributes;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingAgeViewModel
    {
        [Required(ErrorMessage = "Age is required")]
        [Range(10, 120, ErrorMessage = "Please enter a valid age")]
        public int Age { get; set; }
    }
}
