using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class WaterGoals
    {
        [Key]
        public int GoalID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required int WaterGoalMl { get; set; } // Total recommended intake
        public int UserWaterIntake { get; set; } // Actual intake from drinks

        public required bool SetByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Users? User { get; set; }
    }
}
