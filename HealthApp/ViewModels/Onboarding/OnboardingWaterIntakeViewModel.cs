namespace HealthApp.ViewModels.Onboarding
{
    public class OnboardingWaterIntakeViewModel
    {
        public int Water { get; set; }
        public int Soda { get; set; }
        public int DietSoda { get; set; }
        public int Juice { get; set; }
        public int Coffee { get; set; }
        public int Tea { get; set; }
        public int Beer { get; set; }
        public int Wine { get; set; }
        public int SportsDrink { get; set; }
        public int EnergyDrink { get; set; }

        public List<DrinkItem> DrinkTypes => new List<DrinkItem>
        {
            new("Water", "Water", 250),
            new("Soda", "Soda", 330),
            new("DietSoda", "Diet Soda", 330),
            new("Juice", "Fruit Juice", 250),
            new("Coffee", "Coffee", 125),
            new("Tea", "Tea", 125),
            new("Beer", "Beer", 250),
            new("Wine", "Wine", 125),
            new("SportsDrink", "Sport Drink", 500),
            new("EnergyDrink", "Energy Drink", 250),
        };
    }

    public class DrinkItem
    {
        public string Name { get; }
        public string Label { get; }
        public int Volume { get; }

        public DrinkItem(string name, string label, int volume)
        {
            Name = name;
            Label = label;
            Volume = volume;
        }
    }
}
