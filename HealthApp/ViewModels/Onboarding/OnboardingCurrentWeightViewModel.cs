using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingCurrentWeightViewModel
    {
        [Required(ErrorMessage = "Please select your current weight.")]
        [Range(30, 200, ErrorMessage = "Weight must be between 30 and 200 kg.")]
        public float Weight { get; set; }
    }
}
