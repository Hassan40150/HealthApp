using HealthApp.Data;

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
                    // No streak yet ➔ create it
                    _context.Streaks.Add(new Streaks
                    {
                        UserID = userId,
                        Timestamp = today,
                        Completed = true
                    });
                }
                else if (!streak.Completed)
                {
                    // Streak exists but was not completed ➔ update to completed
                    streak.Completed = true;
                    _context.Streaks.Update(streak);
                }
            }
            else
            {
                if (streak != null)
                {
                    // Streak exists but user fell below goal ➔ delete the streak
                    _context.Streaks.Remove(streak);
                }
            }

            await _context.SaveChangesAsync();
        }

    }
}
