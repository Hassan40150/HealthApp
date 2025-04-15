using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingGoalWeightViewModel
    {
        [Required(ErrorMessage = "Please select your goal weight.")]
        [Range(30, 250, ErrorMessage = "Goal weight must be between 30 and 250 kg.")]
        public float GoalWeight { get; set; }
    }
}
