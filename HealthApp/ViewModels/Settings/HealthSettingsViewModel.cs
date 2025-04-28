using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Settings
{
    public class HealthSettingsViewModel
    {
        [Required(ErrorMessage = "Goal weight is required.")]
        [Range(30, 300, ErrorMessage = "Goal weight must be between 30 and 300.")]
        public float GoalWeight { get; set; }

        [Required(ErrorMessage = "Timeline is required.")]
        [Range(3, 24, ErrorMessage = "Timeline must be between 3 and 24 months.")]
        public int TimelineMonths { get; set; }

        public float TDEE { get; set; } // pulled from metrics table
        public float StartingWeight { get; set; } // pulled from userprofiles
        public int RecommendedCalories { get; set; } // calculated live

        [Required(ErrorMessage = "Water goal is required.")]
        [Range(500, 10000, ErrorMessage = "Water goal must be between 500 ml and 10000 ml.")]
        public int WaterGoalMl { get; set; } // input in ml now
    }
}
