using HealthApp.Data;
using HealthApp.Helpers; // Assuming PasswordHelper is here
using HealthApp.Models; // Assuming your User model is here
using Microsoft.EntityFrameworkCore;

namespace HealthApp.Services
{
    public class SettingsService
    {
        private readonly ApplicationDbContext _context;

        public SettingsService(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Check if Email is Available
        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        // 2. Change Password
        public async Task<(bool Success, string? ErrorMessage)> ChangePasswordAsync(int userId, string currentPassword, string newPassword, string confirmPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return (false, "User not found.");

            // 1. Validate current password matches database (after hashing)
            if (user.Password != PasswordHelper.HashPassword(currentPassword))
            {
                return (false, "Current password is incorrect.");
            }

            // 2. Validate new password matches confirm password
            if (newPassword != confirmPassword)
            {
                return (false, "New password and confirm password do not match.");
            }

            // 3. Validate password strength manually (optional, backup if you want double safety)
            if (!System.Text.RegularExpressions.Regex.IsMatch(newPassword, @"^(?=.*[A-Z])(?=.*\d).{8,}$"))
            {
                return (false, "Password must be at least 8 characters long, contain at least one uppercase letter and one number.");
            }

            // 4. Update password with hashing
            user.Password = PasswordHelper.HashPassword(newPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return (true, null);
        }

        // 3. Delete Account
        public async Task DeleteAccountAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserID == userId);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
            }

            // Add to DeletedAccounts table
            var deletedAccounts = new DeletedAccounts
            {
                Username = user.Name,
                Email = user.Email,
                DeletedAt = DateTime.UtcNow
            };
            _context.DeletedAccounts.Add(deletedAccounts);

            _context.Users.Remove(user);

            await _context.SaveChangesAsync();
        }
    }
}
