using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class UserProfiles
    {
        [Key]
        public int ProfileID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required int Age { get; set; }
        public required string Sex { get; set; } // male, female
        public required float HeightCm { get; set; }
        public required float StartingWeight { get; set; }
        public required float GoalWeight { get; set; }
        public required string GoalType { get; set; } // lose, gain, maintain - set by code logic
        public required string ActivityLevel { get; set; }
        public DateTime? GoalTimeline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Users? User { get; set; }
    }
}
