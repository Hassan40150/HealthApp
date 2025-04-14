using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingActivityLevelViewModel
    {
        [Required(ErrorMessage = "Please select your activity level.")]
        [Range(1, 4, ErrorMessage = "Activity level must be between 1 and 4.")]
        public int Level { get; set; }
    }
}
