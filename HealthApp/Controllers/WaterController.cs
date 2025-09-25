using HealthApp.Services;
using HealthApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.Controllers
{
    [Authorize]
    public class WaterController : Controller
    {
        private readonly WaterService _waterService;

        public WaterController(WaterService waterService)
        {
            _waterService = waterService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int range = 0)
        {
            int userId = GetCurrentUserId();

            var model = await _waterService.GetWaterViewModelAsync(userId, (WaterTimeRange)range);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetData(int range)
        {
            int userId = GetCurrentUserId();

            var model = await _waterService.GetWaterViewModelAsync(userId, (WaterTimeRange)range);

            return Json(model);
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
