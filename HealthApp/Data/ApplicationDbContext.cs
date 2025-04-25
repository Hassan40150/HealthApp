namespace HealthApp.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserProfiles> UserProfiles { get; set; }
        public DbSet<WeightLogs> WeightLogs { get; set; }
        public DbSet<CalorieGoals> CalorieGoals { get; set; }
        public DbSet<CalorieLogs> CalorieLogs { get; set; }
        public DbSet<WaterGoals> WaterGoals { get; set; }
        public DbSet<WaterLogs> WaterLogs { get; set; }
        public DbSet<Metrics> Metrics { get; set; }
        public DbSet<Journal> Journal { get; set; }
        public DbSet<Streaks> Streaks { get; set; }


    }
}
