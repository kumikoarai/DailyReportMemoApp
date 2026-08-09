using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DailyReportMemoApp.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkDateToWorkingOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProjects_Companies_CompanyId",
                table: "CompanyProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProjects_Projects_ProjectId",
                table: "CompanyProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskItems_CompanyProjects_CompanyProjectId",
                table: "ProjectTaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskItems_TaskItems_TaskItemId",
                table: "ProjectTaskItems");

            migrationBuilder.DropIndex(
                name: "IX_WorkLogs_WorkDate",
                table: "WorkLogs");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "WorkLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkingOnId",
                table: "WorkLogs",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<DateOnly>(
                name: "WorkDate",
                table: "WorkingOnLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddCheckConstraint(
                name: "CK_WorkLogs_TaskType",
                table: "WorkLogs",
                sql: "(\"ProjectTaskItemId\" IS NOT NULL AND \"SpecialTaskId\" IS NULL)\r\nOR\r\n(\"ProjectTaskItemId\" IS NULL AND \"SpecialTaskId\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingOnLogs_WorkDate",
                table: "WorkingOnLogs",
                column: "WorkDate");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProjects_Companies_CompanyId",
                table: "CompanyProjects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProjects_Projects_ProjectId",
                table: "CompanyProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskItems_CompanyProjects_CompanyProjectId",
                table: "ProjectTaskItems",
                column: "CompanyProjectId",
                principalTable: "CompanyProjects",
                principalColumn: "CompanyProjectId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskItems_TaskItems_TaskItemId",
                table: "ProjectTaskItems",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "TaskItemId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProjects_Companies_CompanyId",
                table: "CompanyProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyProjects_Projects_ProjectId",
                table: "CompanyProjects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskItems_CompanyProjects_CompanyProjectId",
                table: "ProjectTaskItems");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTaskItems_TaskItems_TaskItemId",
                table: "ProjectTaskItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_WorkLogs_TaskType",
                table: "WorkLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkingOnLogs_WorkDate",
                table: "WorkingOnLogs");

            migrationBuilder.DropColumn(
                name: "WorkDate",
                table: "WorkingOnLogs");

            migrationBuilder.AlterColumn<int>(
                name: "WorkingOnId",
                table: "WorkLogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "WorkDate",
                table: "WorkLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkDate",
                table: "WorkLogs",
                column: "WorkDate");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProjects_Companies_CompanyId",
                table: "CompanyProjects",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "CompanyId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyProjects_Projects_ProjectId",
                table: "CompanyProjects",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskItems_CompanyProjects_CompanyProjectId",
                table: "ProjectTaskItems",
                column: "CompanyProjectId",
                principalTable: "CompanyProjects",
                principalColumn: "CompanyProjectId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTaskItems_TaskItems_TaskItemId",
                table: "ProjectTaskItems",
                column: "TaskItemId",
                principalTable: "TaskItems",
                principalColumn: "TaskItemId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
