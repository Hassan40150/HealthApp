using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class WeightLogs
    {
        [Key]
        public int LogID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required float WeightKg { get; set; }
        public required DateTime LogDate { get; set; }

        public Users? User { get; set; }
    }
}
