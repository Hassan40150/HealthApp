using HealthApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using HealthApp.Data;
using HealthApp.Helpers;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string email, string password)
        {
            string hashedPassword = PasswordHelper.HashPassword(password);

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Password == hashedPassword);

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString())
        };

                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("MyCookieAuth", principal);

                // ✅ Fetch the user profile to check onboarding status
                var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == user.UserID);

                if (profile != null && !profile.OnboardingComplete)
                    return RedirectToAction("OnboardingWelcome", "Onboarding");
                else
                    return RedirectToAction("Index", "Dashboard");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Landing");
        }






    }
}
