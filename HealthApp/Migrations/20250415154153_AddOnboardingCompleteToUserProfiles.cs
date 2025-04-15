using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOnboardingCompleteToUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "OnboardingComplete",
                table: "UserProfiles",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OnboardingComplete",
                table: "UserProfiles");
        }
    }
}
