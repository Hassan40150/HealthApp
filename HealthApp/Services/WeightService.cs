using HealthApp.Data;
using HealthApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class WeightService
    {
        private readonly ApplicationDbContext _context;

        public WeightService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WeightViewModel> GetWeightViewModelAsync(int userId, WeightTimeRange range)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
                throw new Exception("Profile not found.");

            var startingWeight = profile.StartingWeight;
            var goalWeight = profile.GoalWeight;
            var createdAt = profile.CreatedAt;

            var rangeStartDate = CalculateRangeStart(range);

            var logsInRange = await _context.WeightLogs
                .Where(w => w.UserID == userId && w.LogDate >= rangeStartDate)
                .OrderBy(w => w.LogDate)
                .ToListAsync();

            var allLogs = await _context.WeightLogs
                .Where(w => w.UserID == userId)
                .OrderBy(w => w.LogDate)
                .ToListAsync();

            var currentWeight = allLogs.LastOrDefault()?.WeightKg ?? startingWeight;
            var progressPercentage = CalculateProgressPercentage(startingWeight, currentWeight, goalWeight);
            var kgRemaining = Math.Abs(goalWeight - currentWeight);

            var heaviest = logsInRange.Any() ? logsInRange.Max(l => l.WeightKg) : currentWeight;
            var lightest = logsInRange.Any() ? logsInRange.Min(l => l.WeightKg) : currentWeight;
            var average = logsInRange.Any() ? logsInRange.Average(l => l.WeightKg) : currentWeight;
            var weightChange = logsInRange.Any() ? logsInRange.Last().WeightKg - logsInRange.First().WeightKg : 0f;

            var entriesLogged = logsInRange.Count;
            var totalDays = (DateTime.UtcNow.Date - rangeStartDate.Date).Days;

            var bmiStart = CalculateBmi(startingWeight, profile.HeightCm);
            var bmiNow = CalculateBmi(currentWeight, profile.HeightCm);

            var estimatedDaysToGoal = EstimateDaysToGoal(startingWeight, currentWeight, goalWeight, createdAt);

            var fluctuation = heaviest - lightest;

            var lastLogDate = allLogs.LastOrDefault()?.LogDate.ToString("yyyy-MM-dd") ?? "No logs";

            return new WeightViewModel
            {
                StartingWeight = startingWeight,
                GoalWeight = goalWeight,
                DaysSinceStart = (DateTime.UtcNow.Date - createdAt.Date).Days,

                CurrentWeight = currentWeight,
                ProgressPercentage = progressPercentage,
                KgRemainingToGoal = kgRemaining,

                HeaviestWeight = heaviest,
                LightestWeight = lightest,
                AverageWeight = average,
                WeightChange = weightChange,
                EntriesLogged = entriesLogged,
                TotalDaysInRange = totalDays,

                EstimatedDaysToGoal = estimatedDaysToGoal,
                BmiChange = $"{bmiStart} → {bmiNow}",
                WeightFluctuation = fluctuation,
                LastLogDate = lastLogDate,

                SelectedRange = range
            };
        }

        private DateTime CalculateRangeStart(WeightTimeRange range)
        {
            return range switch
            {
                WeightTimeRange.OneWeek => DateTime.UtcNow.AddDays(-7),
                WeightTimeRange.TwoWeeks => DateTime.UtcNow.AddDays(-14),
                WeightTimeRange.ThreeMonths => DateTime.UtcNow.AddMonths(-3),
                WeightTimeRange.SixMonths => DateTime.UtcNow.AddMonths(-6),
                WeightTimeRange.TwelveMonths => DateTime.UtcNow.AddMonths(-12),
                WeightTimeRange.TwentyFourMonths => DateTime.UtcNow.AddMonths(-24),
                _ => DateTime.UtcNow.AddDays(-7)
            };
        }

        private float CalculateProgressPercentage(float start, float current, float goal)
        {
            if (start == goal) return 100f;
            return Math.Clamp(((current - start) / (goal - start)) * 100f, 0f, 100f);
        }

        private float CalculateBmi(float weightKg, float heightCm)
        {
            if (heightCm == 0f) return 0f;
            var heightM = heightCm / 100f;
            return (float)Math.Round(weightKg / (heightM * heightM), 1);
        }

        private int EstimateDaysToGoal(float start, float current, float goal, DateTime createdAt)
        {
            var daysElapsed = (DateTime.UtcNow.Date - createdAt.Date).Days;
            var weightLost = Math.Abs(current - start);

            if (weightLost == 0f || daysElapsed == 0)
                return 0;

            var dailyChange = weightLost / daysElapsed;
            var kgRemaining = Math.Abs(goal - current);

            if (dailyChange == 0f)
                return 0;

            return (int)Math.Round(kgRemaining / dailyChange);
        }
    }
}
