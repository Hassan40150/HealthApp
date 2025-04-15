using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using HealthApp.Data;
using HealthApp.Models;



namespace HealthApp.Controllers
{
    public class DashboardController : Controller
    {


        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }



        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Index", "Landing"); // fallback
            }

            int userId = int.Parse(userIdClaim);

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);

            // 🔒 Block users who haven’t completed onboarding
            if (profile == null || !profile.OnboardingComplete)
            {
                return RedirectToAction("OnboardingWelcome", "Onboarding");
            }

            var user = await _context.Users.FindAsync(userId);

            return View(user); // safe to show dashboard now
        }

    }
}
