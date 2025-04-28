namespace HealthApp.Models
{
    public class DeletedAccounts
    {

        [Key]
        public int Id { get; set; }
        public int DeletedAccountID { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime DeletedAt { get; set; }
    }
}