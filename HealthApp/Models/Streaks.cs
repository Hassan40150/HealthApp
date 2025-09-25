using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class Streaks
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public bool Completed { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }

    }
}