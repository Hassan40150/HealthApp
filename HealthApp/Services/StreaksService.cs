using HealthApp.Data;
using HealthApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class StreaksService
    {
        private readonly ApplicationDbContext _context;

        public StreaksService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task MarkStreakIfCompleteAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;

            var streak = await _context.Streaks
                .FirstOrDefaultAsync(s => s.UserID == userId && s.Timestamp == today);

            int totalCalories = await _context.CalorieLogs
                .Where(c => c.UserID == userId && c.LogTime.Date == today)
                .SumAsync(c => (int?)c.Calories) ?? 0;

            float totalWaterMl = (await _context.WaterLogs
                .Where(w => w.UserID == userId && w.LogTime.Date == today)
                .SumAsync(w => (float?)w.AmountLiters) ?? 0f) * 1000f;

            int calorieGoal = await _context.CalorieGoals
                .Where(g => g.UserID == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => g.CalorieGoal)
                .FirstOrDefaultAsync();

            float waterGoalMl = await _context.WaterGoals
                .Where(g => g.UserID == userId)
                .OrderByDescending(g => g.CreatedAt)
                .Select(g => g.WaterGoalMl)
                .FirstOrDefaultAsync();

            bool goalsMet = totalCalories >= calorieGoal && totalWaterMl >= waterGoalMl;

            if (goalsMet)
            {
                if (streak == null)
                {
                    _context.Streaks.Add(new Models.Streaks
                    {
                        UserID = userId,
                        Timestamp = today,
                        Completed = true
                    });
                }
                else if (!streak.Completed)
                {
                    streak.Completed = true;
                    _context.Streaks.Update(streak);
                }
            }
            else
            {
                if (streak != null)
                {
                    _context.Streaks.Remove(streak);
                }
            }

            await _context.SaveChangesAsync();
        }

        // ✅ NEW METHOD (added cleanly)
        public async Task<StreaksViewModel> GetStreaksAsync(int userId)
        {
            var streakDays = await _context.Streaks
                .Where(s => s.UserID == userId && s.Completed)
                .OrderBy(s => s.Timestamp)
                .Select(s => s.Timestamp.Date)
                .ToListAsync();

            if (!streakDays.Any())
            {
                return new StreaksViewModel
                {
                    CurrentStreakLength = 0,
                    LongestStreakLength = 0
                };
            }

            // Calculate longest streak
            int longest = 1;
            int current = 1;
            int maxCurrent = 1;
            DateTime? longestStart = streakDays[0];
            DateTime? longestEnd = streakDays[0];
            DateTime? tempStart = streakDays[0];
            DateTime? currentStart = streakDays[0];

            for (int i = 1; i < streakDays.Count; i++)
            {
                if ((streakDays[i] - streakDays[i - 1]).TotalDays == 1)
                {
                    current++;
                }
                else
                {
                    if (current > longest)
                    {
                        longest = current;
                        longestStart = tempStart;
                        longestEnd = streakDays[i - 1];
                    }
                    current = 1;
                    tempStart = streakDays[i];
                }

                if ((streakDays[i] - streakDays[i - 1]).TotalDays == 1 && streakDays[i] == DateTime.UtcNow.Date)
                {
                    maxCurrent = current;
                    currentStart = tempStart;
                }
            }

            // Final check in case current longest is at end
            if (current > longest)
            {
                longest = current;
                longestStart = tempStart;
                longestEnd = streakDays.Last();
            }

            // Determine if current streak is active today
            bool isCurrentlyStreaking = streakDays.Last() == DateTime.UtcNow.Date;

            return new StreaksViewModel
            {
                CurrentStreakLength = isCurrentlyStreaking ? maxCurrent : 0,
                CurrentStreakStartDate = isCurrentlyStreaking ? currentStart : null,
                LongestStreakLength = longest,
                LongestStreakStartDate = longestStart,
                LongestStreakEndDate = longestEnd
            };
        }
    }
}
