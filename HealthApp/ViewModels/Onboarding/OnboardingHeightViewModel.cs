using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingHeightViewModel
    {
        [Required(ErrorMessage = "Please select your height.")]
        [Range(100, 220, ErrorMessage = "Height must be between 100 cm and 220 cm.")]
        public float HeightCm { get; set; }
    }
}
