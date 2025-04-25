using HealthApp.Models;
using HealthApp.ViewModels.Onboarding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealthApp.Services
{
    public class OnboardingService
    {
        private readonly ApplicationDbContext _context;

        public OnboardingService(ApplicationDbContext context)
        {
            _context = context;
        }

        private int GetUserId(ClaimsPrincipal user)
        {
            var userIdString = user.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(userIdString, out int userId) ? userId : throw new UnauthorizedAccessException();
        }

        public async Task SaveAgeAsync(ClaimsPrincipal user, int age)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                profile = new UserProfiles
                {
                    UserID = userId,
                    Age = age,
                    Sex = "unspecified",
                    HeightCm = 0f,
                    StartingWeight = 0f,
                    GoalWeight = 0f,
                    GoalType = "maintain",
                    ActivityLevel = "unknown",
                    GoalTimeline = 12
                };
                _context.UserProfiles.Add(profile);
            }
            else
            {
                profile.Age = age;
            }

            await _context.SaveChangesAsync();
        }

        public async Task SaveGenderAsync(ClaimsPrincipal user, string sex)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            profile.Sex = sex;
            await _context.SaveChangesAsync();
        }

        public async Task SaveHeightAsync(ClaimsPrincipal user, float heightCm)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            profile.HeightCm = heightCm;
            await _context.SaveChangesAsync();
        }

        public async Task SaveStartingWeightAsync(ClaimsPrincipal user, float weight)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            profile.StartingWeight = weight;
            await _context.SaveChangesAsync();
        }

        public async Task SaveGoalWeightAsync(ClaimsPrincipal user, float goalWeight)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            profile.GoalWeight = goalWeight;
            profile.GoalType = goalWeight < profile.StartingWeight ? "lose" :
                                goalWeight > profile.StartingWeight ? "gain" : "maintain";

            await _context.SaveChangesAsync();
        }

        public async Task SaveActivityLevelAsync(ClaimsPrincipal user, int level)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            string[] activityLevels = { "low", "occasional", "moderate", "high" };
            profile.ActivityLevel = activityLevels[level - 1];

            await _context.SaveChangesAsync();
        }

        public async Task SaveWaterIntakeAsync(ClaimsPrincipal user, OnboardingWaterIntakeViewModel model)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) throw new InvalidOperationException("Profile not found.");

            float weight = profile.StartingWeight;
            int baseGoal = (int)(weight * 35);

            int activityBonus = profile.ActivityLevel.ToLower() switch
            {
                "moderate" => 300,
                "high" => 500,
                _ => 0
            };

            int recommendedGoal = baseGoal + activityBonus;

            var waterSources = new Dictionary<string, (int volumeMl, double contribution)>
            {
                { "Water", (250, 1.0) },
                { "Soda", (330, 0.9) },
                { "DietSoda", (330, 0.9) },
                { "Juice", (250, 0.9) },
                { "Coffee", (125, 0.95) },
                { "Tea", (125, 0.95) },
                { "Beer", (250, 0.92) },
                { "Wine", (125, 0.85) },
                { "SportsDrink", (500, 0.9) },
                { "EnergyDrink", (250, 0.85) }
            };

            var drinkCounts = new Dictionary<string, int>
            {
                { "Water", model.Water },
                { "Soda", model.Soda },
                { "DietSoda", model.DietSoda },
                { "Juice", model.Juice },
                { "Coffee", model.Coffee },
                { "Tea", model.Tea },
                { "Beer", model.Beer },
                { "Wine", model.Wine },
                { "SportsDrink", model.SportsDrink },
                { "EnergyDrink", model.EnergyDrink }
            };

            int totalEffectiveWaterMl = drinkCounts.Sum(d =>
            {
                var (volume, contribution) = waterSources[d.Key];
                return (int)(d.Value * volume * contribution);
            });

            var waterGoal = new WaterGoals
            {
                UserID = userId,
                WaterGoalMl = recommendedGoal,
                UserWaterIntake = totalEffectiveWaterMl,
                SetByUser = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.WaterGoals.Add(waterGoal);
            await _context.SaveChangesAsync();
        }

        public async Task FinalizeMetricsAsync(ClaimsPrincipal user)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) return;

            float heightMeters = profile.HeightCm / 100f;
            float weight = profile.StartingWeight;

            float bmi = weight / (heightMeters * heightMeters);

            float bmr = profile.Sex.ToLower() switch
            {
                "male" => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age + 5,
                "female" => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age - 161,
                _ => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age
            };

            float activityFactor = profile.ActivityLevel.ToLower() switch
            {
                "low" => 1.2f,
                "occasional" => 1.375f,
                "moderate" => 1.55f,
                "high" => 1.725f,
                _ => 1.2f
            };

            float tdee = bmr * activityFactor;

            var metrics = new Metrics
            {
                UserID = userId,
                BMI = (float)Math.Round(bmi, 1),
                BMR = (int)Math.Round(bmr),
                TDEE = (int)Math.Round(tdee),
                EstimatedTimeToGoalDays = 365,
                LastUpdated = DateTime.UtcNow
            };
            _context.Metrics.Add(metrics);

            int calorieGoal = profile.GoalType.ToLower() switch
            {
                "lose" => (int)(tdee - 500),
                "gain" => (int)(tdee + 300),
                _ => (int)tdee
            };

            var calGoal = new CalorieGoals
            {
                UserID = userId,
                CalorieGoal = calorieGoal,
                SetByUser = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.CalorieGoals.Add(calGoal);

            var weightLog = new WeightLogs
            {
                UserID = userId,
                WeightKg = weight,
                LogDate = DateTime.UtcNow
            };
            _context.WeightLogs.Add(weightLog);

            await _context.SaveChangesAsync();
        }

        public async Task CompleteOnboardingAsync(ClaimsPrincipal user)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile != null)
            {
                profile.OnboardingComplete = true;
                await _context.SaveChangesAsync();
            }
        }

        public int CalculateCalorieGoal(int tdee, float startWeight, float goalWeight, int months, string goalType)
        {
            float weightDiff = Math.Abs(goalWeight - startWeight);
            float totalCalories = weightDiff * 7700f;
            float dailyAdjustment = totalCalories / (months * 30f);

            return goalType.ToLower() switch
            {
                "lose" => (int)(tdee - dailyAdjustment),
                "gain" => (int)(tdee + dailyAdjustment),
                _ => tdee
            };
        }

        public async Task<Walkthrough3ViewModel> PrepareWalkthrough3ViewModelAsync(ClaimsPrincipal user)
        {
            int userId = GetUserId(user);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            var waterGoal = await _context.WaterGoals.FirstOrDefaultAsync(w => w.UserID == userId);

            if (profile == null || waterGoal == null) throw new InvalidOperationException();

            float surfaceAreaFactor = (profile.HeightCm * 0.007f);
            float baseLossPerKg = 20f;

            float baselineLoss = profile.StartingWeight * baseLossPerKg;
            float adjustedBaseline = baselineLoss * surfaceAreaFactor;

            float activityMultiplier = profile.ActivityLevel.ToLower() switch
            {
                "low" => 1.0f,
                "occasional" => 1.1f,
                "moderate" => 1.3f,
                "high" => 1.5f,
                _ => 1.0f
            };

            int typicalLoss = (int)(adjustedBaseline * activityMultiplier);

            string feedback = waterGoal.UserWaterIntake < waterGoal.WaterGoalMl
                ? $"Careful! You're drinking less than recommended. Try to drink more water daily."
                : $"Well done! You're meeting your hydration goal.";

            return new Walkthrough3ViewModel
            {
                RecommendedMl = waterGoal.WaterGoalMl,
                ActualIntakeMl = waterGoal.UserWaterIntake,
                TypicalLossMl = typicalLoss,
                HydrationFeedback = feedback
            };
        }




        public async Task<UserProfiles?> GetUserProfileAsync(int userId)
        {
            return await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
        }

        public async Task<Metrics?> GetUserMetricsAsync(int userId)
        {
            return await _context.Metrics.FirstOrDefaultAsync(m => m.UserID == userId);
        }

        public async Task UpdateGoalTimelineAndCaloriesAsync(ClaimsPrincipal user, int timelineMonths, int recommendedCalories)
        {
            int userId = GetUserId(user);

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile != null)
            {
                profile.GoalTimeline = timelineMonths;
            }

            var calGoal = await _context.CalorieGoals.FirstOrDefaultAsync(c => c.UserID == userId);
            if (calGoal != null)
            {
                calGoal.CalorieGoal = recommendedCalories;
            }

            await _context.SaveChangesAsync();
        }




    }
}
