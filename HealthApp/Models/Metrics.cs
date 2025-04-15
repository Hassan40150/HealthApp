using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class Metrics
    {
        [Key]
        public int MetricID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public float BMI { get; set; }
        public float BMR { get; set; }
        public float TDEE { get; set; }
        public int EstimatedTimeToGoalDays { get; set; }
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public Users? User { get; set; }
    }
}
