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
                return RedirectToAction("Index", "Landing"); // or show an error view
            }

            int userId = int.Parse(userIdClaim);


            var user = await _context.Users.FindAsync(userId);

            return View(user); // Pass the user to the view
        }
    }
}
