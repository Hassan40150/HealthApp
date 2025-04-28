using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthApp.ViewModels
{
    public enum CaloriesTimeRange
    {
        OneWeek = 0,
        TwoWeeks = 1,
        ThreeMonths = 2,
        SixMonths = 3,
        TwelveMonths = 4,
        TwentyFourMonths = 5
    }

    public class CaloriesViewModel
    {
        // Today’s Data
        public int TodaysCalories { get; set; }
        public float TodaysProgressPercentage { get; set; }

        // Range-Based Data
        public float AverageIntake { get; set; }
        public int HighestIntake { get; set; }
        public int LowestIntake { get; set; }

        public int NetDeficitSurplus { get; set; } // Could be negative or positive
        public float PredictedWeeklyWeightChange { get; set; } // in kg
        public int EstimatedDaysToGoal { get; set; }

        // Goal Adherence
        public float GoalAdherencePercentage { get; set; }
        public int DaysMetGoal { get; set; }
        public int DaysUnderGoal { get; set; }
        public string LastMissedGoalDay { get; set; } = string.Empty;

        // UI
        public CaloriesTimeRange SelectedRange { get; set; }
    }
}
