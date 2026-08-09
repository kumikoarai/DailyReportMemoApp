using DailyReportMemoApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace DailyReportMemoApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Company> Companies { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<TaskItem> TaskItems { get; set; }

        public DbSet<CompanyProject> CompanyProjects { get; set; }

        public DbSet<ProjectTaskItem> ProjectTaskItems { get; set; }

        public DbSet<SpecialTask> SpecialTasks { get; set; }

        public DbSet<WorkLog> WorkLogs { get; set; }

        public DbSet<WorkTimeRange> WorkTimeRanges { get; set; }

        public DbSet<WorkingOn> WorkingOnLogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            var folderPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "DailyReportMemoApp"
            );

            Directory.CreateDirectory(folderPath);

            var dbPath = Path.Combine(folderPath, "DailyReportMemo.db");

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>(entity =>
            {
                entity.ToTable("Companies");

                entity.HasKey(x => x.CompanyId);

                entity.Property(x => x.CompanyName)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasIndex(x => x.CompanyName);
            });

            modelBuilder.Entity<Project>(entity =>
            {
                entity.ToTable("Projects");

                entity.HasKey(x => x.ProjectId);

                entity.Property(x => x.ProjectName)
                      .IsRequired();

                entity.Property(x => x.Completed)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasIndex(x => x.ProjectName);
            });

            modelBuilder.Entity<TaskItem>(entity =>
            {
                entity.ToTable("TaskItems");

                entity.HasKey(x => x.TaskItemId);

                entity.Property(x => x.TaskItemName)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasIndex(x => x.TaskItemName);
            });

            modelBuilder.Entity<CompanyProject>(entity =>
            {
                entity.ToTable("CompanyProjects");

                entity.HasKey(x => x.CompanyProjectId);

                entity.Property(x => x.CompanyId)
                      .IsRequired();

                entity.Property(x => x.ProjectId)
                     .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasOne(x => x.Companies)
                      .WithMany(x => x.CompanyProjects)
                      .HasForeignKey(x => x.CompanyId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.Projects)
                      .WithMany(x => x.CompanyProjects)
                      .HasForeignKey(x => x.ProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.CompanyId, x.ProjectId })
                      .IsUnique();
            });

            modelBuilder.Entity<ProjectTaskItem>(entity =>
            {
                entity.ToTable("ProjectTaskItems");

                entity.HasKey(x => x.ProjectTaskItemId);

                entity.Property(x => x.CompanyProjectId)
                      .IsRequired();

                entity.Property(x => x.TaskItemId)
                      .IsRequired();

                entity.Property(x => x.IsCurrent)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasOne(x => x.CompanyProjects)
                      .WithMany(x => x.ProjectTaskItems)
                      .HasForeignKey(x => x.CompanyProjectId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.TaskItems)
                      .WithMany(x => x.ProjectTaskItems)
                      .HasForeignKey(x => x.TaskItemId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(x => new { x.CompanyProjectId, x.TaskItemId })
                      .IsUnique();
            });

            modelBuilder.Entity<SpecialTask>(entity =>
            {
                entity.ToTable("SpecialTasks");

                entity.HasKey(x => x.SpecialTaskId);

                entity.Property(x => x.SpecialTaskName)
                      .IsRequired();

                entity.Property(x => x.DefaultStartFlg)
                      .IsRequired();

                entity.Property(x => x.IsDeleted)
                      .IsRequired();

                entity.Property(x => x.IsActive)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

            });

            modelBuilder.Entity<WorkLog>(entity =>
            {
                entity.ToTable("WorkLogs", table =>
                {
                    table.HasCheckConstraint(
                        "CK_WorkLogs_TaskType",
                        """
                        ("ProjectTaskItemId" IS NOT NULL AND "SpecialTaskId" IS NULL)
                        OR
                        ("ProjectTaskItemId" IS NULL AND "SpecialTaskId" IS NOT NULL)
                        """
                    );
                });

                entity.HasKey(x => x.WorkLogId);

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasOne(x => x.ProjectTaskItems)
                      .WithMany(x => x.WorkLogs)
                      .HasForeignKey(x => x.ProjectTaskItemId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.SpecialTasks)
                      .WithMany(x => x.WorkLogs)
                      .HasForeignKey(x => x.SpecialTaskId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(x => x.WorkingOnLogs)
                      .WithMany(x => x.WorkLogs)
                      .HasForeignKey(x => x.WorkingOnId)
                      .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<WorkTimeRange>(entity =>
            {
                entity.ToTable("WorkTimeRanges");

                entity.HasKey(x => x.WorkTimeRangeId);

                entity.Property(x => x.WorkLogId)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasOne(x => x.WorkLogs)
                      .WithMany(x => x.WorkTimeRanges)
                      .HasForeignKey(x => x.WorkLogId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<WorkingOn>(entity =>
            {
                entity.ToTable("WorkingOnLogs");

                entity.HasKey(x => x.WorkingOnId);

                entity.Property(x => x.WorkingOnFlg)
                      .IsRequired();

                entity.Property(x => x.WorkDate)
                      .IsRequired();

                entity.Property(x => x.WorkingOnStart)
                      .IsRequired();

                entity.Property(x => x.CreatedAt)
                      .IsRequired();

                entity.HasIndex(x => x.WorkDate);
            });

        }
    }
}