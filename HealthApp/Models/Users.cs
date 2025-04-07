namespace HealthApp.Models
{
    public class Users
    {
        [Key]
        public int UserID { get; set; }

        public required string Name { get; set; }

        public int Age { get; set; }

        public required string Gender { get; set; }




    }
}
