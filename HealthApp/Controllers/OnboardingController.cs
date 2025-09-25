using HealthApp.Attributes;
using HealthApp.Models;
using HealthApp.Services;
using HealthApp.ViewModels;
using HealthApp.ViewModels.Onboarding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.Controllers
{
    public class OnboardingController : Controller
    {
        private readonly OnboardingService _onboardingService;

        public OnboardingController(OnboardingService onboardingService)
        {
            _onboardingService = onboardingService;
        }

        [OnboardingRequired]
        public IActionResult OnboardingWelcome()
        {
            ViewBag.UserName = User.FindFirstValue(ClaimTypes.Name);
            return View("OnboardingWelcome");
        }

        [OnboardingRequired]
        public IActionResult OnboardingTrackIntro()
        {
            return View("OnboardingTrackIntro");
        }

        [OnboardingRequired]
        public IActionResult OnboardingSetupIntro()
        {
            return View("OnboardingSetupIntro");
        }

        [OnboardingRequired]
        public IActionResult OnboardingAge()
        {
            return View("OnboardingAge");
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingAge(OnboardingAgeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveAgeAsync(User, model.Age);
            return RedirectToAction("OnboardingGender");
        }

        [OnboardingRequired]
        public IActionResult OnboardingGender()
        {
            return View("OnboardingGender");
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingGender(OnboardingGenderViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveGenderAsync(User, model.Sex);
            return RedirectToAction("OnboardingHeight");
        }

        [OnboardingRequired]
        public IActionResult OnboardingHeight()
        {
            var viewModel = new OnboardingHeightViewModel { HeightCm = 170 };
            return View("OnboardingHeight", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingHeight(OnboardingHeightViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveHeightAsync(User, model.HeightCm);
            return RedirectToAction("OnboardingCurrentWeight");
        }

        [OnboardingRequired]
        public IActionResult OnboardingCurrentWeight()
        {
            var viewModel = new OnboardingCurrentWeightViewModel { Weight = 70 };
            return View("OnboardingCurrentWeight", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingCurrentWeight(OnboardingCurrentWeightViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveStartingWeightAsync(User, model.Weight);
            return RedirectToAction("OnboardingGoalWeight");
        }

        [OnboardingRequired]
        public IActionResult OnboardingGoalWeight()
        {
            var viewModel = new OnboardingGoalWeightViewModel { GoalWeight = 70 };
            return View("OnboardingGoalWeight", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingGoalWeight(OnboardingGoalWeightViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveGoalWeightAsync(User, model.GoalWeight);
            return RedirectToAction("OnboardingActivityLevel");
        }

        [OnboardingRequired]
        public IActionResult OnboardingActivityLevel()
        {
            var viewModel = new OnboardingActivityLevelViewModel { Level = 1 };
            return View("OnboardingActivityLevel", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingActivityLevel(OnboardingActivityLevelViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveActivityLevelAsync(User, model.Level);
            return RedirectToAction("OnboardingWaterIntake");
        }

        [OnboardingRequired]
        public IActionResult OnboardingWaterIntake()
        {
            var viewModel = new OnboardingWaterIntakeViewModel();
            return View("OnboardingWaterIntake", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingWaterIntake(OnboardingWaterIntakeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.SaveWaterIntakeAsync(User, model);
            await _onboardingService.FinalizeMetricsAsync(User);

            return RedirectToAction("OnboardingWalkthrough3");
        }

        [OnboardingRequired]
        public IActionResult OnboardingWalkthrough1()
        {
            return View("OnboardingWalkthrough1");
        }

        [HttpPost]
        public async Task<IActionResult> CompleteOnboarding()
        {
            await _onboardingService.CompleteOnboardingAsync(User);
            return RedirectToAction("Index", "Dashboard");
        }

        [OnboardingRequired]
        public async Task<IActionResult> OnboardingWalkthrough2()
        {
            int userId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : throw new UnauthorizedAccessException();
            var profile = await _onboardingService.GetUserProfileAsync(userId);
            var metrics = await _onboardingService.GetUserMetricsAsync(userId);

            if (profile == null || metrics == null)
                return RedirectToAction("OnboardingAge");

            var model = new Walkthrough2ViewModel
            {
                TDEE = (int)metrics.TDEE,
                StartingWeight = profile.StartingWeight,
                GoalWeight = profile.GoalWeight,
                GoalType = profile.GoalType,
                TimelineMonths = 12,
                RecommendedCalories = _onboardingService.CalculateCalorieGoal(
                    (int)metrics.TDEE,
                    profile.StartingWeight,
                    profile.GoalWeight,
                    12,
                    profile.GoalType)
            };

            return View("OnboardingWalkthrough2", model);
        }

        [HttpPost]
        public async Task<IActionResult> OnboardingWalkthrough2(Walkthrough2ViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            await _onboardingService.UpdateGoalTimelineAndCaloriesAsync(User, model.TimelineMonths, model.RecommendedCalories);
            return RedirectToAction("OnboardingWalkthrough1");
        }

        [OnboardingRequired]
        public async Task<IActionResult> OnboardingWalkthrough3()
        {
            var model = await _onboardingService.PrepareWalkthrough3ViewModelAsync(User);
            return View("OnboardingWalkthrough3", model);
        }
    }
}
