using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyReportMemoApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "SpecialTasks",
                columns: table => new
                {
                    SpecialTaskId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SpecialTaskName = table.Column<string>(type: "TEXT", nullable: false),
                    DefaultStartFlg = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialTasks", x => x.SpecialTaskId);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                columns: table => new
                {
                    TaskItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TaskItemName = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskItems", x => x.TaskItemId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProjects",
                columns: table => new
                {
                    CompanyProjectId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Memo = table.Column<string>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProjects", x => x.CompanyProjectId);
                    table.ForeignKey(
                        name: "FK_CompanyProjects_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyProjects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTaskItems",
                columns: table => new
                {
                    ProjectTaskItemId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCurrent = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTaskItems", x => x.ProjectTaskItemId);
                    table.ForeignKey(
                        name: "FK_ProjectTaskItems_CompanyProjects_CompanyProjectId",
                        column: x => x.CompanyProjectId,
                        principalTable: "CompanyProjects",
                        principalColumn: "CompanyProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectTaskItems_TaskItems_TaskItemId",
                        column: x => x.TaskItemId,
                        principalTable: "TaskItems",
                        principalColumn: "TaskItemId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    WorkLogId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ProjectTaskItemId = table.Column<int>(type: "INTEGER", nullable: true),
                    SpecialTaskId = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.WorkLogId);
                    table.ForeignKey(
                        name: "FK_WorkLogs_ProjectTaskItems_ProjectTaskItemId",
                        column: x => x.ProjectTaskItemId,
                        principalTable: "ProjectTaskItems",
                        principalColumn: "ProjectTaskItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_WorkLogs_SpecialTasks_SpecialTaskId",
                        column: x => x.SpecialTaskId,
                        principalTable: "SpecialTasks",
                        principalColumn: "SpecialTaskId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WorkTimeRanges",
                columns: table => new
                {
                    WorkTimeRangeId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    WorkLogId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    EndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkTimeRanges", x => x.WorkTimeRangeId);
                    table.ForeignKey(
                        name: "FK_WorkTimeRanges_WorkLogs_WorkLogId",
                        column: x => x.WorkLogId,
                        principalTable: "WorkLogs",
                        principalColumn: "WorkLogId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CompanyName",
                table: "Companies",
                column: "CompanyName");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProjects_CompanyId_ProjectId",
                table: "CompanyProjects",
                columns: new[] { "CompanyId", "ProjectId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProjects_ProjectId",
                table: "CompanyProjects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectName",
                table: "Projects",
                column: "ProjectName");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskItems_CompanyProjectId_TaskItemId",
                table: "ProjectTaskItems",
                columns: new[] { "CompanyProjectId", "TaskItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTaskItems_TaskItemId",
                table: "ProjectTaskItems",
                column: "TaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TaskItemName",
                table: "TaskItems",
                column: "TaskItemName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_ProjectTaskItemId",
                table: "WorkLogs",
                column: "ProjectTaskItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_SpecialTaskId",
                table: "WorkLogs",
                column: "SpecialTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkDate",
                table: "WorkLogs",
                column: "WorkDate");

            migrationBuilder.CreateIndex(
                name: "IX_WorkTimeRanges_WorkLogId",
                table: "WorkTimeRanges",
                column: "WorkLogId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkTimeRanges");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "ProjectTaskItems");

            migrationBuilder.DropTable(
                name: "SpecialTasks");

            migrationBuilder.DropTable(
                name: "CompanyProjects");

            migrationBuilder.DropTable(
                name: "TaskItems");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
