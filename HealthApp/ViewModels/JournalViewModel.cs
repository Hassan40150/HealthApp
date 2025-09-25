using System;
using System.Collections.Generic;

namespace HealthApp.ViewModels
{
    public class JournalViewModel
    {
        public int EntriesTodayCount { get; set; }
        public int MaxEntriesPerDay { get; set; } = 3;

        public List<JournalEntryItem> Entries { get; set; } = new List<JournalEntryItem>();
    }

    public class JournalEntryItem
    {
        public DateTime Timestamp { get; set; }
        public string Entry { get; set; }
    }
}
