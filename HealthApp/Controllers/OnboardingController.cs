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
        public IActionResult OnboardingAge()
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
        public IActionResult OnboardingGender()
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
        public IActionResult OnboardingHeight()
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




    }
}
