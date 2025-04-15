using System.ComponentModel.DataAnnotations;
using HealthApp.Attributes;

namespace HealthApp.ViewModels.Onboarding
{
    public class Walkthrough2ViewModel
    {
        public int TDEE { get; set; }

        public float StartingWeight { get; set; }
        public float GoalWeight { get; set; }
        public required string GoalType { get; set; }

        public int TimelineMonths { get; set; } = 12; // Default to 12 months

        public int RecommendedCalories { get; set; }
    }
}
