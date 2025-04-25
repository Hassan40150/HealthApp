using System.ComponentModel.DataAnnotations.Schema;

namespace HealthApp.Models
{
    public class Journal
    {
        [Key]
        public int Id { get; set; }

        public DateTime Timestamp { get; set; }

        public required string Entry { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
    }
}