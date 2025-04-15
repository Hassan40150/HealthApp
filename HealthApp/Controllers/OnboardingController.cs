using HealthApp.Attributes;
using HealthApp.Models;
using HealthApp.ViewModels;
using HealthApp.ViewModels.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace HealthApp.Controllers
{
    public class OnboardingController : Controller
    {


        private readonly ApplicationDbContext _context;

        public OnboardingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [OnboardingRequired]

        public IActionResult OnboardingWelcome() // 1
        {
            return View("OnboardingWelcome");
        }

        [OnboardingRequired]

        public IActionResult OnboardingTrackIntro() // 2
        {
            return View("OnboardingTrackIntro");
        }

        [OnboardingRequired]

        public IActionResult OnboardingSetupIntro() // 3
        {
            return View("OnboardingSetupIntro");
        }


        // GET: OnboardingAge
        [OnboardingRequired]

        public IActionResult OnboardingAge() // 4
        {
            return View("OnboardingAge");
        }

        // POST: OnboardingAge
        [HttpPost]
        public async Task<IActionResult> OnboardingAge(OnboardingAgeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                profile = new UserProfiles // define dummy values
                {
                    UserID = userId,
                    Age = model.Age,
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
                profile.Age = model.Age;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingGender");
        }


        // GET: OnboardingGender
        [OnboardingRequired]

        public IActionResult OnboardingGender() // 5
        {
            return View("OnboardingGender");
        }

        // POST: OnboardingGender
        [HttpPost]
        public async Task<IActionResult> OnboardingGender(OnboardingGenderViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                // Option A: Redirect them back to the Age step
                return RedirectToAction("OnboardingAge");

                // Option B (fallback): Create profile again
                // Not recommended if you're tracking onboarding flow strictly
            }

            profile.Sex = model.Sex;
            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingHeight");
        }

        // GET: OnboardingHeight
        [OnboardingRequired]

        public IActionResult OnboardingHeight() // 6
        {
            var viewModel = new OnboardingHeightViewModel
            {
                HeightCm = 170 // Default slider position (optional)
            };

            return View("OnboardingHeight", viewModel);
        }

        // POST: OnboardingHeight
        [HttpPost]
        public async Task<IActionResult> OnboardingHeight(OnboardingHeightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                return RedirectToAction("OnboardingAge"); // Fallback if profile isn't created
            }

            profile.HeightCm = model.HeightCm;
            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingCurrentWeight");
        }


        // GET: OnboardingCurrentWeight
        [OnboardingRequired]

        public IActionResult OnboardingCurrentWeight() // 7
        {
            var viewModel = new OnboardingCurrentWeightViewModel
            {
                Weight = 70 // Default slider position (optional)
            };

            return View("OnboardingCurrentWeight", viewModel);
        }

        // POST: OnboardingCurrentWeight
        [HttpPost]
        public async Task<IActionResult> OnboardingCurrentWeight(OnboardingCurrentWeightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                return RedirectToAction("OnboardingAge"); // Safety fallback
            }

            profile.StartingWeight = model.Weight;
            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingGoalWeight");
        }


        // GET: OnboardingGoalWeight
        [OnboardingRequired]

        public IActionResult OnboardingGoalWeight()
        {
            var viewModel = new OnboardingGoalWeightViewModel
            {
                GoalWeight = 70 // Default slider position (optional)
            };

            return View("OnboardingGoalWeight", viewModel);
        }

        // POST: OnboardingGoalWeight
        [HttpPost]
        public async Task<IActionResult> OnboardingGoalWeight(OnboardingGoalWeightViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                return RedirectToAction("OnboardingAge"); // Safety fallback
            }

            profile.GoalWeight = model.GoalWeight;

            // Optional: set GoalType based on logic
            if (profile.GoalWeight < profile.StartingWeight)
                profile.GoalType = "lose";
            else if (profile.GoalWeight > profile.StartingWeight)
                profile.GoalType = "gain";
            else
                profile.GoalType = "maintain";

            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingActivityLevel");
        }


        // GET: OnboardingActivityLevel
        [OnboardingRequired]

        public IActionResult OnboardingActivityLevel()
        {
            var viewModel = new OnboardingActivityLevelViewModel
            {
                Level = 1 // Default slider position (Low)
            };

            return View("OnboardingActivityLevel", viewModel);
        }

        // POST: OnboardingActivityLevel
        [HttpPost]
        public async Task<IActionResult> OnboardingActivityLevel(OnboardingActivityLevelViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
            {
                return RedirectToAction("OnboardingAge"); // fallback
            }

            // Map numeric level to text description
            string[] activityLevels = { "low", "occasional", "moderate", "high" };
            profile.ActivityLevel = activityLevels[model.Level - 1];

            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingWaterIntake");
        }


        // GET: OnboardingWaterIntake
        [OnboardingRequired]

        public IActionResult OnboardingWaterIntake()
        {
            var viewModel = new OnboardingWaterIntakeViewModel();
            return View("OnboardingWaterIntake", viewModel);
        }

        // POST: OnboardingWaterIntake
        [HttpPost]
        public async Task<IActionResult> OnboardingWaterIntake(OnboardingWaterIntakeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized();
            }

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null)
            {
                return RedirectToAction("OnboardingAge");
            }

            // === Step 1: Calculate total recommended water goal (mL) ===
            float weight = profile.StartingWeight;
            int baseGoal = (int)(weight * 35);

            int activityBonus = profile.ActivityLevel.ToLower() switch
            {
                "moderate" => 300,
                "high" => 500,
                _ => 0
            };

            int recommendedGoal = baseGoal + activityBonus;

            // === Step 2: Calculate actual user intake from drinks ===
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

            // === Step 3: Save both values to WaterGoals ===
            var waterGoal = new WaterGoals
            {
                UserID = userId,
                WaterGoalMl = recommendedGoal,
                UserWaterIntake = totalEffectiveWaterMl,
                SetByUser = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.WaterGoals.Add(waterGoal);

            // Optional: pass intake for use in the summary view
            TempData["ActualWaterIntake"] = totalEffectiveWaterMl;

            await _context.SaveChangesAsync();


            await FinalizeUserMetricsAsync(userId);

            return RedirectToAction("OnboardingWalkthrough1");
        }


        // Final Calculations
        private async Task FinalizeUserMetricsAsync(int userId)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile == null) return;

            float heightMeters = profile.HeightCm / 100f;
            float weight = profile.StartingWeight;

            // === Calculate BMI ===
            float bmi = weight / (heightMeters * heightMeters);

            // === Calculate BMR (Mifflin-St Jeor) ===
            float bmr = profile.Sex.ToLower() switch
            {
                "male" => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age + 5,
                "female" => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age - 161,
                _ => 10 * weight + 6.25f * profile.HeightCm - 5 * profile.Age
            };

            // === Calculate TDEE ===
            float activityFactor = profile.ActivityLevel.ToLower() switch
            {
                "low" => 1.2f,
                "occasional" => 1.375f,
                "moderate" => 1.55f,
                "high" => 1.725f,
                _ => 1.2f
            };

            float tdee = bmr * activityFactor;

            // === Estimate time to goal (days) ===
            float weightDiff = Math.Abs(profile.GoalWeight - profile.StartingWeight);
            int estimatedDays = 365; // default 12 months

            // === Save to Metrics table ===
            var metrics = new Metrics
            {
                UserID = userId,
                BMI = (float)Math.Round(bmi, 1),
                BMR = (int)Math.Round(bmr),
                TDEE = (int)Math.Round(tdee),
                EstimatedTimeToGoalDays = estimatedDays,
                LastUpdated = DateTime.UtcNow
            };
            _context.Metrics.Add(metrics);

            // === Calculate Calorie Goal ===
            int calorieGoal = profile.GoalType.ToLower() switch
            {
                "lose" => (int)(tdee - 500),
                "gain" => (int)(tdee + 300),
                _ => (int)tdee
            };

            // === Save to CalorieGoals table ===
            var calGoal = new CalorieGoals
            {
                UserID = userId,
                CalorieGoal = calorieGoal,
                SetByUser = false,
                CreatedAt = DateTime.UtcNow
            };
            _context.CalorieGoals.Add(calGoal);

            // === Save initial weight log ===
            var weightLog = new WeightLogs
            {
                UserID = userId,
                WeightKg = weight,
                LogDate = DateTime.UtcNow
            };
            _context.WeightLogs.Add(weightLog);

            await _context.SaveChangesAsync();
        }


        // GET: OnboardingWalkthrough1
        [OnboardingRequired]

        public IActionResult OnboardingWalkthrough1()
        {
            return View("OnboardingWalkthrough1");
        }



        // GET: OnboardingWalkthrough2
        [OnboardingRequired]

        public async Task<IActionResult> OnboardingWalkthrough2()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            var metrics = await _context.Metrics.FirstOrDefaultAsync(m => m.UserID == userId);

            if (profile == null || metrics == null)
                return RedirectToAction("OnboardingAge");

            var model = new Walkthrough2ViewModel
            {
                TDEE = (int)metrics.TDEE,
                StartingWeight = profile.StartingWeight,
                GoalWeight = profile.GoalWeight,
                GoalType = profile.GoalType,
                TimelineMonths = 12
            };

            // Precalculate 12-month goal
            model.RecommendedCalories = CalculateCalorieGoal(
                model.TDEE,
                model.StartingWeight,
                model.GoalWeight,
                model.TimelineMonths,
                model.GoalType
            );

            return View("OnboardingWalkthrough2", model);
        }

        // POST: OnboardingWalkthrough2
        [HttpPost]
        public async Task<IActionResult> OnboardingWalkthrough2(Walkthrough2ViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
                return Unauthorized();

            // Recalculate updated calorie goal
            int calorieGoal = CalculateCalorieGoal(
                model.TDEE,
                model.StartingWeight,
                model.GoalWeight,
                model.TimelineMonths,
                model.GoalType
            );

            // === Update existing CalorieGoals row ===
            var calGoal = await _context.CalorieGoals.FirstOrDefaultAsync(c => c.UserID == userId);
            if (calGoal != null)
            {
                calGoal.CalorieGoal = calorieGoal;
                // Don't touch SetByUser or CreatedAt
            }

            // === Update timeline in UserProfiles ===
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile != null)
            {
                profile.GoalTimeline = model.TimelineMonths;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("OnboardingWalkthrough3");
        }


        // HELPER METHOD FOR WALKTHROUGH 2
        private int CalculateCalorieGoal(int tdee, float startWeight, float goalWeight, int months, string goalType)
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

        // GET: OnboardingWalkthrough3
        [OnboardingRequired]

        public async Task<IActionResult> OnboardingWalkthrough3()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            var waterGoal = await _context.WaterGoals.FirstOrDefaultAsync(w => w.UserID == userId);

            if (profile == null || waterGoal == null)
                return RedirectToAction("OnboardingAge");

            // CALC LOSS
            float surfaceAreaFactor = (profile.HeightCm * 0.007f); // height contributes slightly
            float baseLossPerKg = 20f; // ml per kg baseline metabolic + respiratory + passive loss

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
                ? "Careful! You're drinking less than recommended. Try to drink more water daily."
                : "Well done! You're meeting your hydration goal. We'll help you stay on track by logging your intake.";

            var model = new Walkthrough3ViewModel
            {
                RecommendedMl = waterGoal.WaterGoalMl,
                ActualIntakeMl = waterGoal.UserWaterIntake,
                TypicalLossMl = typicalLoss,
                HydrationFeedback = feedback
            };

            return View("OnboardingWalkthrough3", model);
        }

        // POST: OnboardingWalkthrough3

        [HttpPost]
        public async Task<IActionResult> OnboardingWalkthrough3Post()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile != null)
            {
                profile.OnboardingComplete = true;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", "Dashboard");
        }

    }
}
