using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyReportMemoApp.Repositories
{
    public class ProjectsRepository
    {
        /// <summary>
        /// 案件のリストを取得するメソッド
        /// </summary>
        /// <returns></returns>
        public List<Project> GetAllProjects()
        {
            using (var db = new AppDbContext())
            {
                return db.Projects
                       .ToList();
            };

        }

        /// <summary>
        /// 案件を追加するメソッド
        /// </summary>
        /// <param name="project"></param>
        public Project AddProject(AppDbContext db, Project project)
        {
            db.Projects.Add(project);
            db.SaveChanges();
            return project;
        }

        /// <summary>
        /// 案件名を更新するメソッド
        /// </summary>
        /// <param name="db"></param>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="now"></param>
        /// <returns></returns>
        public void UpdateProjectName(AppDbContext db, int projectId, String projectName, bool completedProject, DateTime now)
        {
            var project = db.Projects
                .FirstOrDefault(x => x.ProjectId == projectId);

            if (project == null)
            {
                MessageBox.Show("更新対象の案件データが見つかりませんでした。");
                return;
            }

            project.ProjectName = projectName;
            project.Completed = completedProject;
            project.UpdatedAt = now;
        }
    }
}
