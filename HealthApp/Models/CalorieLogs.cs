using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class CalorieLogs
    {
        [Key]
        public int LogID { get; set; }

        [ForeignKey("Users")]
        public int UserID { get; set; }

        public required int Calories { get; set; }
        public required DateTime LogTime { get; set; }

        public Users? User { get; set; }
    }
}
