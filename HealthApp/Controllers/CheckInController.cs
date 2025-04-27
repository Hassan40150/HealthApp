using Microsoft.AspNetCore.Mvc;
using HealthApp.ViewModels;
using HealthApp.Services; // assuming services live here
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;


namespace HealthApp.Controllers
{
    [Authorize]
    public class CheckinController : Controller
    {
        private readonly CheckInService _checkInService;


        public CheckinController(CheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var model = await _checkInService.GetTodayCheckInDataAsync(UserId());
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitCheckin(CheckInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            await _checkInService.SubmitCheckInAsync(UserId(), model);
            return RedirectToAction("Index");
        }

        private int UserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? throw new Exception("User ID claim not found."));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCalories(int amount)
        {
            await _checkInService.AddCaloriesAsync(UserId(), amount);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWater(int amount)
        {
            await _checkInService.AddWaterAsync(UserId(), amount);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateWeight(float weight)
        {
            await _checkInService.UpdateWeightAsync(UserId(), weight);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCheckInData()
        {
            var data = await _checkInService.GetTodayCheckInDataAsync(UserId());
            return Json(data);
        }

    }
}
