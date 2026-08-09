using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Project AddProject(Project project)
        {
            using (var db = new AppDbContext())
            {
                db.Projects.Add(project);
                db.SaveChanges();
            }
            return project;
        }

    }
}
