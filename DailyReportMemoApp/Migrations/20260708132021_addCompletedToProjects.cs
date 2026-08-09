using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyReportMemoApp.Migrations
{
    /// <inheritdoc />
    public partial class addCompletedToProjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Completed",
                table: "Projects",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Completed",
                table: "Projects");
        }
    }
}
