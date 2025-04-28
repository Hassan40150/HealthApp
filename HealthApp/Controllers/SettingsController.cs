using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using HealthApp.Data;
using HealthApp.Services;
using HealthApp.ViewModels;
using HealthApp.ViewModels.Settings;
using HealthApp.Models;

namespace HealthApp.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly SettingsService _settingsService;

        public SettingsController(ApplicationDbContext context, SettingsService settingsService)
        {
            _context = context;
            _settingsService = settingsService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new Exception("User ID not found."));
        }

        /* MAIN SETTINGS PAGE */

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (user == null || profile == null)
                return NotFound();

            var model = new SettingsViewModel
            {
                Username = user.Name,
                Email = user.Email,
                Age = profile.Age,
                HeightCm = (int)profile.HeightCm,
                Sex = profile.Sex
            };

            return View(model);
        }

        /* ACCOUNT SETTINGS PAGE */

        [HttpGet]
        public async Task<IActionResult> AccountSettings()
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            var model = new AccountSettingsViewModel
            {
                Username = user.Name,
                Email = user.Email
            };

            return View("AccountSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> AccountSettings(AccountSettingsViewModel model)
        {
            var userId = GetUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return NotFound();

            // 1. Update Username
            if (!string.IsNullOrWhiteSpace(model.Username) && model.Username != user.Name)
            {
                user.Name = model.Username;
            }

            // 2. Update Email
            if (!string.IsNullOrWhiteSpace(model.Email) && model.Email != user.Email)
            {
                var emailAvailable = await _settingsService.IsEmailAvailableAsync(model.Email);
                if (!emailAvailable)
                {
                    ModelState.AddModelError(nameof(model.Email), "Email is already in use.");
                    return View("AccountSettings", model);
                }

                user.Email = model.Email;
            }

            // 3. Change Password
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                var passwordResult = await _settingsService.ChangePasswordAsync(userId, model.CurrentPassword!, model.NewPassword!, model.ConfirmPassword!);

                if (!passwordResult.Success)
                {
                    if (passwordResult.ErrorMessage == "Current password is incorrect.")
                    {
                        ModelState.AddModelError(nameof(model.CurrentPassword), passwordResult.ErrorMessage);
                    }
                    else if (passwordResult.ErrorMessage == "New password and confirm password do not match.")
                    {
                        ModelState.AddModelError(nameof(model.ConfirmPassword), passwordResult.ErrorMessage);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, passwordResult.ErrorMessage ?? "Password change failed.");
                    }

                    return View("AccountSettings", model);
                }
            }

            // 4. Delete Account
            if (model.ConfirmDelete)
            {
                await _settingsService.DeleteAccountAsync(userId);

                await HttpContext.SignOutAsync(); // Log out
                return RedirectToAction("Index", "Home");
            }

            // Save updates
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Account settings updated successfully.";
            return RedirectToAction("AccountSettings");
        }



        /* USER SETTINGS */

        [HttpGet]
        public async Task<IActionResult> UserSettings()
        {
            var userId = GetUserId();
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
                return NotFound();

            var model = new UserSettingsViewModel
            {
                Age = profile.Age,
                HeightCm = (int)profile.HeightCm,
                Sex = profile.Sex
            };

            return View("UserSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> UserSettings(UserSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("UserSettings", model);
            }

            var userId = GetUserId();
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            if (profile == null)
                return NotFound();

            // Optional: Extract this logic into service if you prefer
            profile.Age = model.Age;
            profile.HeightCm = model.HeightCm;
            profile.Sex = model.Sex;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "User settings updated successfully.";
            return RedirectToAction("UserSettings");
        }


        /* HEALTH SETTINGS */

        [HttpGet]
        public async Task<IActionResult> HealthSettings()
        {
            var userId = GetUserId();
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            var metrics = await _context.Metrics.FirstOrDefaultAsync(m => m.UserID == userId);
            var waterGoal = await _context.WaterGoals.FirstOrDefaultAsync(w => w.UserID == userId);

            if (profile == null || metrics == null || waterGoal == null)
                return NotFound();

            var model = new HealthSettingsViewModel
            {
                GoalWeight = profile.GoalWeight,
                TimelineMonths = (int)profile.GoalTimeline,
                TDEE = metrics.TDEE,
                StartingWeight = profile.StartingWeight,
                RecommendedCalories = 0, // Calculated live in view
                WaterGoalMl = waterGoal.WaterGoalMl
            };

            return View("HealthSettings", model);
        }

        [HttpPost]
        public async Task<IActionResult> HealthSettings(HealthSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View("HealthSettings", model);

            var userId = GetUserId();
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            var metrics = await _context.Metrics.FirstOrDefaultAsync(m => m.UserID == userId);
            var waterGoal = await _context.WaterGoals.FirstOrDefaultAsync(w => w.UserID == userId);

            if (profile == null || metrics == null || waterGoal == null)
                return NotFound();

            // Update user profile
            profile.GoalWeight = model.GoalWeight;
            profile.GoalTimeline = model.TimelineMonths;

            // Recalculate calories server side
            var weightDiff = Math.Abs(profile.GoalWeight - profile.StartingWeight);
            var totalCaloriesChange = weightDiff * 7700;
            var dailyAdjustment = totalCaloriesChange / (profile.GoalTimeline * 30);

            var newCalories = metrics.TDEE;

            if (profile.GoalWeight < profile.StartingWeight)
                newCalories = (int)(metrics.TDEE - dailyAdjustment);
            else if (profile.GoalWeight > profile.StartingWeight)
                newCalories = (int)(metrics.TDEE + dailyAdjustment);

            newCalories = Math.Max(0, (int)Math.Round(newCalories));

            // Insert new calorie goal record
            _context.CalorieGoals.Add(new CalorieGoals
            {
                UserID = userId,
                CalorieGoal = (int)newCalories,
                SetByUser = false,
                CreatedAt = DateTime.UtcNow
            });

            // Update water goal
            waterGoal.WaterGoalMl = model.WaterGoalMl;
            waterGoal.SetByUser = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Health settings updated successfully.";
            return RedirectToAction("HealthSettings");
        }




    }
}
