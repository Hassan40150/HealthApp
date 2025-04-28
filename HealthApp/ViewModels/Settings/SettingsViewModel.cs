using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Settings
{
    public class SettingsViewModel
    {

        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public int HeightCm { get; set; }
        public string Sex { get; set; } = string.Empty;

    }
}
