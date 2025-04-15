using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class WaterLogs
    {
        [Key]
        public int LogID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required float AmountLiters { get; set; }
        public required DateTime LogTime { get; set; }

        public Users? User { get; set; }
    }
}
