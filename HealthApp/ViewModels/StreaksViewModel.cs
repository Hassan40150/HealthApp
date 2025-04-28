using System;

namespace HealthApp.ViewModels
{
    public class StreaksViewModel
    {
        public int CurrentStreakLength { get; set; }
        public DateTime? CurrentStreakStartDate { get; set; }

        public int LongestStreakLength { get; set; }
        public DateTime? LongestStreakStartDate { get; set; }
        public DateTime? LongestStreakEndDate { get; set; }
    }
}
