using System;

namespace HealthApp.ViewModels
{
    public enum WaterTimeRange
    {
        OneWeek = 0,
        TwoWeeks = 1,
        ThreeMonths = 2,
        SixMonths = 3,
        TwelveMonths = 4,
        TwentyFourMonths = 5
    }

    public class WaterViewModel
    {
        // Today’s Data
        public int TodaysWaterIntake { get; set; }
        public float TodaysProgressPercentage { get; set; }

        // Range-Based Data
        public float AverageWaterIntake { get; set; }
        public int HighestIntake { get; set; }
        public int LowestIntake { get; set; }

        // Static Metrics
        public int RecommendedHydrationGoal { get; set; }
        public int TypicalWaterLossPerDay { get; set; }

        // UI
        public WaterTimeRange SelectedRange { get; set; }
    }
}
