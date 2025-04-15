using System;
using HealthApp.Helpers;
using HealthApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.EntityFrameworkCore;




namespace HealthApp.Controllers
{
    public class RegisterController : Controller
    {

        private readonly ApplicationDbContext _context;

        public RegisterController(ApplicationDbContext context)
        {
            _context = context;
        }



        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Users
                {
                    Name = model.Name,
                    Password = PasswordHelper.HashPassword(model.Password),
                    Email = model.Email
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // ✅ Create empty UserProfile after saving user
                var profile = new UserProfiles
                {
                    UserID = user.UserID,
                    Age = 0,
                    Sex = "unspecified",
                    HeightCm = 0,
                    StartingWeight = 0,
                    GoalWeight = 0,
                    GoalType = "maintain",
                    ActivityLevel = "unknown",
                    OnboardingComplete = false
                };

                _context.UserProfiles.Add(profile);
                await _context.SaveChangesAsync();

                // 🔐 Log the user in
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.Name)
        };

                var identity = new ClaimsIdentity(claims, "login");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(principal);

                // 🔁 Redirect to onboarding
                return RedirectToAction("OnboardingWelcome", "Onboarding");
            }

            return View("Index", model);
        }

    }
}
