using System.Security.Claims;
using HealthApp.Services;
using HealthApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HealthApp.Controllers
{
    [Authorize]
    public class WeightController : Controller
    {
        private readonly WeightService _weightService;

        public WeightController(WeightService weightService)
        {
            _weightService = weightService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int range = 0)
        {
            int userId = GetCurrentUserId();

            var model = await _weightService.GetWeightViewModelAsync(userId, (WeightTimeRange)range);

            return View(model);
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


        [HttpGet]
        public async Task<IActionResult> GetData(int range)
        {
            int userId = GetCurrentUserId();
            var model = await _weightService.GetWeightViewModelAsync(userId, (WeightTimeRange)range);
            return Json(model);
        }


    }
}
