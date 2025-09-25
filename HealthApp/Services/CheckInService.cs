using HealthApp.Models;
using HealthApp.ViewModels;
using HealthApp.Models; // Assuming your ViewModels and database models are here
using HealthApp.Data; // Assuming your DbContext is here
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class CheckInService
    {
        private readonly ApplicationDbContext _context;
        private readonly StreaksService _streaksService;

        public CheckInService(ApplicationDbContext context, StreaksService streaksService)
        {
            _context = context;
            _streaksService = streaksService;
        }

        public async Task<CheckInViewModel> GetTodayCheckInDataAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var latestWeight = await _context.WeightLogs
                    .Where(w => w.UserID == userId)
                    .OrderByDescending(w => w.LogDate) // now uses full date + time
                    .Select(w => (float?)w.WeightKg)
                    .FirstOrDefaultAsync();

            if (latestWeight == null)
            {
                // fallback to starting weight
                latestWeight = await _context.UserProfiles
                    .Where(p => p.UserID == userId)
                    .Select(p => (float?)p.StartingWeight)
                    .FirstOrDefaultAsync();
            }

            var totalCalories = await _context.CalorieLogs
                .Where(c => c.UserID == userId && c.LogTime.Date == today)
                .SumAsync(c => (int?)c.Calories) ?? 0;

            var totalWater = await _context.WaterLogs
                .Where(w => w.UserID == userId && w.LogTime.Date == today)
                .SumAsync(w => (float?)w.AmountLiters) ?? 0f;

            var calorieGoal = await _context.CalorieGoals
                .Where(g => g.UserID == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => g.CalorieGoal)
                .FirstOrDefaultAsync();

            var waterGoal = await _context.WaterGoals
                .Where(g => g.UserID == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => g.WaterGoalMl)
                .FirstOrDefaultAsync();

            return new CheckInViewModel
            {
                Weight = latestWeight,
                Calories = totalCalories,
                Water = totalWater,
                CalorieGoal = calorieGoal,
                WaterGoal = waterGoal
            };
        }

        public async Task SubmitCheckInAsync(int userId, CheckInViewModel model)
        {
            var today = DateTime.UtcNow.Date;
            var now = DateTime.UtcNow;

            if (model.Weight.HasValue && model.Weight.Value > 0)
            {
                _context.WeightLogs.Add(new WeightLogs
                {
                    UserID = userId,
                    WeightKg = model.Weight.Value,
                    LogDate = today
                });
            }

            if (model.Calories.HasValue && model.Calories.Value > 0)
            {
                _context.CalorieLogs.Add(new CalorieLogs
                {
                    UserID = userId,
                    Calories = model.Calories.Value,
                    LogTime = now
                });
            }

            if (model.Water.HasValue && model.Water.Value > 0)
            {
                _context.WaterLogs.Add(new WaterLogs
                {
                    UserID = userId,
                    AmountLiters = model.Water.Value,
                    LogTime = now
                });
            }

            await _context.SaveChangesAsync();

            // After any new check-in, attempt to mark the streak
            await _streaksService.MarkStreakIfCompleteAsync(userId);
        }

        public async Task AddCaloriesAsync(int userId, int amount)
        {
            _context.CalorieLogs.Add(new CalorieLogs
            {
                UserID = userId,
                Calories = amount,
                LogTime = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // ADD THIS
            await _streaksService.MarkStreakIfCompleteAsync(userId);
        }

        public async Task AddWaterAsync(int userId, int amount)
        {
            _context.WaterLogs.Add(new WaterLogs
            {
                UserID = userId,
                AmountLiters = amount / 1000f, // assuming your DB stores liters
                LogTime = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();

            // ADD THIS
            await _streaksService.MarkStreakIfCompleteAsync(userId);
        }


        public async Task UpdateWeightAsync(int userId, float weight)
        {
            _context.WeightLogs.Add(new WeightLogs
            {
                UserID = userId,
                WeightKg = weight,
                LogDate = DateTime.UtcNow // full time + date
            });

            await _context.SaveChangesAsync();
        }

    }
}