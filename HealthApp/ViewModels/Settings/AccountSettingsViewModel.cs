using System.ComponentModel.DataAnnotations;

namespace HealthApp.ViewModels.Settings
{
    public class AccountSettingsViewModel
    {
        // Prefilled, optional fields
        public string? Username { get; set; } // Current username

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string? Email { get; set; }     // Email, optional

        // Password change fields
        [RequiredIf("NewPassword", ErrorMessage = "Current password is required when changing password.")]
        public string? CurrentPassword { get; set; }

        [RequiredIf("CurrentPassword", ErrorMessage = "New password is required when changing password.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain at least one uppercase letter, and one number.")]
        public string? NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }

        // For account deletion confirmation
        public bool ConfirmDelete { get; set; }
    }
}
