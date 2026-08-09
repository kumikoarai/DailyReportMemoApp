using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyReportMemoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkingOnLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WorkingOnId",
                table: "WorkLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WorkingOnLogs",
                columns: table => new
                {
                    WorkingOnId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkingOnFlg = table.Column<bool>(type: "INTEGER", nullable: false),
                    WorkingOnStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WorkingOnEnd = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingOnLogs", x => x.WorkingOnId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkingOnId",
                table: "WorkLogs",
                column: "WorkingOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkLogs_WorkingOnLogs_WorkingOnId",
                table: "WorkLogs",
                column: "WorkingOnId",
                principalTable: "WorkingOnLogs",
                principalColumn: "WorkingOnId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkLogs_WorkingOnLogs_WorkingOnId",
                table: "WorkLogs");

            migrationBuilder.DropTable(
                name: "WorkingOnLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_WorkingOnId",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "WorkingOnId",
                table: "WorkLogs");
        }
    }
}
