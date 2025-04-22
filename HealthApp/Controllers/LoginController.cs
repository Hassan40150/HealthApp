using HealthApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HealthApp.Data;
using HealthApp.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HealthApp.ViewModels;

namespace HealthApp.Controllers
{

    public class LoginController : Controller
    {

        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpGet]
        public IActionResult Index()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model); // Show "email required", etc.

            string hashedPassword = PasswordHelper.HashPassword(model.Password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Invalid credentials");
                return View(model); // 🔥 this must pass `model` back
            }

            // Authentication logic
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
    };

            var identity = new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("MyCookieAuth", principal);

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == user.UserID);
            if (profile != null && !profile.OnboardingComplete)
                return RedirectToAction("OnboardingWelcome", "Onboarding");

            return RedirectToAction("Index", "Dashboard");
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Landing");
        }


    }
}
