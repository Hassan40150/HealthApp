using System.ComponentModel.DataAnnotations.Schema;


namespace HealthApp.Models
{

    public class Users
    {
        [Key]
        public int UserID { get; set; }

        public required string Name { get; set; }

        public required string Password { get; set; }

        [EmailAddress]
        public required string Email { get; set; }

    }
}
