using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class UserProfiles
    {
        [Key]
        public int ProfileID { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; } // FK to Users table

        public required int Age { get; set; }
        public required string Sex { get; set; } // e.g., male, female
        public required float HeightCm { get; set; }
        public required float StartingWeight { get; set; }
        public required float GoalWeight { get; set; }
        public required string GoalType { get; set; } // lose, gain, maintain
        public required string ActivityLevel { get; set; }
        public int? GoalTimeline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Users? User { get; set; }

        public UserProfiles()
        {
            Age = 0;
            Sex = "unspecified";
            HeightCm = 0f;
            StartingWeight = 0f;
            GoalWeight = 0f;
            GoalType = "maintain";
            ActivityLevel = "unknown";
        }
    }
}
