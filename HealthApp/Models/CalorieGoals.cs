using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class CalorieGoals
    {
        [Key]
        public int GoalID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required int CalorieGoal { get; set; }
        public required bool SetByUser { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Users? User { get; set; }
    }
}
