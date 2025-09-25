namespace HealthApp.ViewModels
{
    public class CheckInViewModel
    {
        public float? Weight { get; set; }
        public int? Calories { get; set; }
        public float? Water { get; set; }
        public bool CheckInComplete { get; set; }
        public int CalorieGoal { get; set; }
        public float WaterGoal { get; set; }

    }
}
