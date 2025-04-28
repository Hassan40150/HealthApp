using HealthApp.Data;
using HealthApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class CaloriesService
    {
        private readonly ApplicationDbContext _context;

        public CaloriesService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CaloriesViewModel> GetCaloriesViewModelAsync(int userId, CaloriesTimeRange range)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId);

            var calorieGoalEntry = await _context.CalorieGoals
                .FirstOrDefaultAsync(cg => cg.UserID == userId);

            if (profile == null || calorieGoalEntry == null)
                throw new Exception("Profile or Calorie Goal not found.");

            var today = DateTime.UtcNow.Date;
            var rangeStartDate = CalculateRangeStart(range);

            float calorieGoal = calorieGoalEntry.CalorieGoal;

            // Fetch today's calorie log
            var todayLog = await _context.CalorieLogs
                .FirstOrDefaultAsync(c => c.UserID == userId && c.LogTime == today);

            int todaysCalories = todayLog?.Calories ?? 0;
            float todaysProgressPercentage = calorieGoal > 0
                ? (todaysCalories / calorieGoal) * 100f
                : 0f;

            // Fetch logs in selected range
            var logsInRange = await _context.CalorieLogs
                .Where(c => c.UserID == userId && c.LogTime >= rangeStartDate)
                .ToListAsync();

            float averageIntake = logsInRange.Any()
                ? (float)logsInRange.Average(l => l.Calories)
                : 0f;

            int highestIntake = logsInRange.Any()
                ? logsInRange.Max(l => l.Calories)
                : 0;

            int lowestIntake = logsInRange.Any()
                ? logsInRange.Min(l => l.Calories)
                : 0;

            // ⚠️ TDEE assumed 0 for now until you confirm
            int tdee = 0;
            int netDeficitSurplus = (tdee > 0 && logsInRange.Any())
                ? (int)(logsInRange.Sum(l => l.Calories) - (tdee * logsInRange.Count))
                : 0;

            float predictedWeeklyWeightChange = (netDeficitSurplus / 7700f) * -1f; // 7700 kcal = 1kg

            int estimatedDaysToGoal = CalculateEstimatedDaysToGoal(
                profile.StartingWeight, profile.GoalWeight, predictedWeeklyWeightChange
            );

            int daysMetGoal = logsInRange.Count(l => Math.Abs(l.Calories - calorieGoal) <= calorieGoal * 0.1f);
            int daysUnderGoal = logsInRange.Count(l => l.Calories < calorieGoal * 0.9f);

            float goalAdherencePercentage = logsInRange.Any()
                ? (daysMetGoal / (float)logsInRange.Count) * 100f
                : 0f;

            var lastMissedGoal = logsInRange
                .Where(l => Math.Abs(l.Calories - calorieGoal) > calorieGoal * 0.1f)
                .OrderByDescending(l => l.LogTime)
                .FirstOrDefault()?.LogTime.ToString("yyyy-MM-dd") ?? "No missed days";

            return new CaloriesViewModel
            {
                TodaysCalories = todaysCalories,
                TodaysProgressPercentage = todaysProgressPercentage,

                AverageIntake = averageIntake,
                HighestIntake = highestIntake,
                LowestIntake = lowestIntake,

                NetDeficitSurplus = netDeficitSurplus,
                PredictedWeeklyWeightChange = predictedWeeklyWeightChange,
                EstimatedDaysToGoal = estimatedDaysToGoal,

                GoalAdherencePercentage = goalAdherencePercentage,
                DaysMetGoal = daysMetGoal,
                DaysUnderGoal = daysUnderGoal,
                LastMissedGoalDay = lastMissedGoal,

                SelectedRange = range
            };
        }

        private DateTime CalculateRangeStart(CaloriesTimeRange range)
        {
            return range switch
            {
                CaloriesTimeRange.OneWeek => DateTime.UtcNow.AddDays(-7),
                CaloriesTimeRange.TwoWeeks => DateTime.UtcNow.AddDays(-14),
                CaloriesTimeRange.ThreeMonths => DateTime.UtcNow.AddMonths(-3),
                CaloriesTimeRange.SixMonths => DateTime.UtcNow.AddMonths(-6),
                CaloriesTimeRange.TwelveMonths => DateTime.UtcNow.AddMonths(-12),
                CaloriesTimeRange.TwentyFourMonths => DateTime.UtcNow.AddMonths(-24),
                _ => DateTime.UtcNow.AddDays(-7)
            };
        }

        private int CalculateEstimatedDaysToGoal(float startingWeight, float goalWeight, float predictedWeeklyChange)
        {
            if (predictedWeeklyChange == 0)
                return 0;

            float kgRemaining = Math.Abs(goalWeight - startingWeight);
            float weeksNeeded = kgRemaining / Math.Abs(predictedWeeklyChange);
            return (int)Math.Round(weeksNeeded * 7); // Convert weeks to days
        }
    }
}
