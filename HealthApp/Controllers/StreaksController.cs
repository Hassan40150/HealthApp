using HealthApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.Controllers
{
    [Authorize]
    public class StreaksController : Controller
    {
        private readonly StreaksService _streaksService;

        public StreaksController(StreaksService streaksService)
        {
            _streaksService = streaksService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var viewModel = await _streaksService.GetStreaksAsync(userId);
            return View(viewModel); // ✅ Pass ViewModel to View
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            throw new Exception("User ID not found in claims!");
        }
    }
}
