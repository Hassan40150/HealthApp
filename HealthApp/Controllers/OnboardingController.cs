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



        public IActionResult OnboardingWelcome() // 1
        {
            return View("OnboardingWelcome");
        }

        public IActionResult OnboardingTrackIntro() // 2
        {
            return View("OnboardingTrackIntro");
        }


        public IActionResult OnboardingSetupIntro() // 3
        {
            return View("OnboardingSetupIntro");
        }


        // GET: OnboardingAge
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

            return RedirectToAction("OnboardingCalculatedSummary");
        }


        // POST: Final Calculations
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
    }
}
