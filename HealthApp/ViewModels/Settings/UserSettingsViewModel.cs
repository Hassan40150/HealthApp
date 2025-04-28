using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Settings
{
    public class UserSettingsViewModel
    {
        [Range(13, 120, ErrorMessage = "Age must be between 13 and 120.")]
        public int Age { get; set; }

        [Range(100, 220, ErrorMessage = "Height must be between 100 cm and 220 cm.")]
        public int HeightCm { get; set; }

        [Required(ErrorMessage = "Sex is required.")]
        [RegularExpression("^(Male|Female)$", ErrorMessage = "Sex must be either Male or Female.")]
        public string Sex { get; set; } = string.Empty;
    }
}
