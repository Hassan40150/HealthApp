using HealthApp.Data;
using HealthApp.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class JournalService
    {
        private readonly ApplicationDbContext _context;

        public JournalService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetEntriesTodayCountAsync(int userId)
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Journal
                .CountAsync(j => j.UserID == userId && j.Timestamp.Date == today);
        }

        public async Task<List<JournalEntryItem>> GetJournalEntriesAsync(int userId, int skip = 0, int take = 10)
        {
            return await _context.Journal
                .Where(j => j.UserID == userId)
                .OrderByDescending(j => j.Timestamp)
                .Skip(skip)
                .Take(take)
                .Select(j => new JournalEntryItem
                {
                    Timestamp = j.Timestamp,
                    Entry = j.Entry
                })
                .ToListAsync();
        }

        public async Task<bool> AddJournalEntryAsync(int userId, string entryText)
        {
            var today = DateTime.UtcNow.Date;
            int entriesToday = await _context.Journal
                .CountAsync(j => j.UserID == userId && j.Timestamp.Date == today);

            if (entriesToday >= 3)
            {
                return false; // Already reached limit
            }

            var newEntry = new Models.Journal
            {
                UserID = userId,
                Timestamp = DateTime.UtcNow,
                Entry = entryText
            };

            _context.Journal.Add(newEntry);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
