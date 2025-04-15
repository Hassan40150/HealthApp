using System.ComponentModel.DataAnnotations;
using HealthApp.Attributes;

namespace HealthApp.ViewModels.Onboarding
{
    public class Walkthrough3ViewModel
    {
        public int RecommendedMl { get; set; }
        public int ActualIntakeMl { get; set; }
        public int TypicalLossMl { get; set; }

        public string HydrationFeedback { get; set; } = "";
    }
}
