using HealthApp.Attributes;
using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Onboarding
{
    public class Walkthrough2ViewModel
    {
        [Required]
        public int TDEE { get; set; }

        [Required]
        public float StartingWeight { get; set; }

        [Required]
        public float GoalWeight { get; set; }

        [Required]
        public string GoalType { get; set; } = "maintain";

        [Range(3, 24)]
        public int TimelineMonths { get; set; } = 12;

        [Required]
        public int RecommendedCalories { get; set; }
    }
}
