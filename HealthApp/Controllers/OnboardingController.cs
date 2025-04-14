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
                    ActivityLevel = "unknown"
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



    }
}
