using HealthApp.Services;
using HealthApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealthApp.Controllers
{
    [Authorize]
    public class JournalController : Controller
    {
        private readonly JournalService _journalService;

        public JournalController(JournalService journalService)
        {
            _journalService = journalService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();

            var viewModel = new JournalViewModel
            {
                EntriesTodayCount = await _journalService.GetEntriesTodayCountAsync(userId),
                Entries = await _journalService.GetJournalEntriesAsync(userId, 0, 10)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddEntry(string entryText)
        {
            int userId = GetCurrentUserId();

            if (string.IsNullOrWhiteSpace(entryText) || entryText.Length > 280)
            {
                return BadRequest("Entry must be between 1 and 280 characters.");
            }

            bool success = await _journalService.AddJournalEntryAsync(userId, entryText);

            if (!success)
            {
                return BadRequest("Daily journal entry limit reached.");
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetEntries(int skip = 0, int take = 10)
        {
            int userId = GetCurrentUserId();
            var entries = await _journalService.GetJournalEntriesAsync(userId, skip, take);

            return Json(entries);
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
