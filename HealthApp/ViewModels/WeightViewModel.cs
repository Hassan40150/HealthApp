using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace HealthApp.ViewModels
{
    public enum WeightTimeRange
    {
        OneWeek = 0,
        TwoWeeks = 1,
        ThreeMonths = 2,
        SixMonths = 3,
        TwelveMonths = 4,
        TwentyFourMonths = 5
    }

    public class WeightViewModel
    {
        // Profile & Goal
        public float StartingWeight { get; set; }
        public float GoalWeight { get; set; }
        public int DaysSinceStart { get; set; }

        // Current Status
        public float CurrentWeight { get; set; }
        public float ProgressPercentage { get; set; }
        public float KgRemainingToGoal { get; set; }

        // Range-Based Stats
        public float HeaviestWeight { get; set; }
        public float LightestWeight { get; set; }
        public float AverageWeight { get; set; }
        public float WeightChange { get; set; }
        public int EntriesLogged { get; set; }
        public int TotalDaysInRange { get; set; }

        // Additional Metrics
        public int EstimatedDaysToGoal { get; set; }
        public string BmiChange { get; set; } = string.Empty; // Example "28 → 26"
        public float WeightFluctuation { get; set; }
        public string LastLogDate { get; set; } = string.Empty;

        // UI Support
        public WeightTimeRange SelectedRange { get; set; }
    }
}
