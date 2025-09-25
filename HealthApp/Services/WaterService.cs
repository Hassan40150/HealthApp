using HealthApp.Data;
using HealthApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class WaterService
    {
        private readonly ApplicationDbContext _context;

        public WaterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<WaterViewModel> GetWaterViewModelAsync(int userId, WaterTimeRange range)
        {
            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserID == userId);

            var waterGoalEntry = await _context.WaterGoals
                .FirstOrDefaultAsync(wg => wg.UserID == userId);

            if (profile == null || waterGoalEntry == null)
                throw new Exception("Profile or Water Goal not found.");

            var today = DateTime.UtcNow.Date;
            var rangeStartDate = CalculateRangeStart(range);

            int waterGoal = waterGoalEntry.WaterGoalMl;

            // ✅ Sum all water logs for today
            float todaysWaterIntakeLiters = await _context.WaterLogs
                .Where(w => w.UserID == userId && w.LogTime.Date == today)
                .SumAsync(w => w.AmountLiters);

            // ✅ Convert liters to milliliters
            int todaysWaterIntake = (int)(todaysWaterIntakeLiters * 1000f);

            float todaysProgressPercentage = waterGoal > 0
                ? (todaysWaterIntake / (float)waterGoal) * 100f
                : 0f;

            // Fetch logs in selected range
            var logsInRange = await _context.WaterLogs
                .Where(w => w.UserID == userId && w.LogTime >= rangeStartDate)
                .ToListAsync();

            // Calculate stats over the range
            float averageWaterIntakeMl = logsInRange.Any()
                ? (float)logsInRange.Average(l => l.AmountLiters) * 1000f
                : 0f;

            int highestIntakeMl = logsInRange.Any()
                ? (int)(logsInRange.Max(l => l.AmountLiters) * 1000f)
                : 0;

            int lowestIntakeMl = logsInRange.Any()
                ? (int)(logsInRange.Min(l => l.AmountLiters) * 1000f)
                : 0;

            // Static typical water loss for now
            int typicalWaterLossPerDay = 2500; // (mL)

            return new WaterViewModel
            {
                TodaysWaterIntake = todaysWaterIntake,
                TodaysProgressPercentage = todaysProgressPercentage,

                AverageWaterIntake = averageWaterIntakeMl,
                HighestIntake = highestIntakeMl,
                LowestIntake = lowestIntakeMl,

                RecommendedHydrationGoal = waterGoal,
                TypicalWaterLossPerDay = typicalWaterLossPerDay,

                SelectedRange = range
            };
        }

        private DateTime CalculateRangeStart(WaterTimeRange range)
        {
            return range switch
            {
                WaterTimeRange.OneWeek => DateTime.UtcNow.AddDays(-7),
                WaterTimeRange.TwoWeeks => DateTime.UtcNow.AddDays(-14),
                WaterTimeRange.ThreeMonths => DateTime.UtcNow.AddMonths(-3),
                WaterTimeRange.SixMonths => DateTime.UtcNow.AddMonths(-6),
                WaterTimeRange.TwelveMonths => DateTime.UtcNow.AddMonths(-12),
                WaterTimeRange.TwentyFourMonths => DateTime.UtcNow.AddMonths(-24),
                _ => DateTime.UtcNow.AddDays(-7)
            };
        }
    }
}
