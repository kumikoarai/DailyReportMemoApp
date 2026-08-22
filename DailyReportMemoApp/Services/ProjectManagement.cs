using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Services
{
    public class ProjectManagement
    {
        /// <summary>
        /// 案件情報を更新するメソッド
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="projectName"></param>
        /// <param name="completedProject"></param>
        /// <param name="companyProjectId"></param>
        /// <param name="memo"></param>
        public bool ProjectUpdate(int projectId, String projectName, bool completedProject, int companyProjectId, String memo)
        {
            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();
            var now = DateTime.Now;

            try
            {
                var projectsRepository = new ProjectsRepository();
                var companyProjectsRepository = new CompanyProjectsRepository();

                // 案件名と完了フラグを更新
                projectsRepository.UpdateProjectName(db, projectId, projectName, completedProject, now);

                // メモを更新
                companyProjectsRepository.UpdateCompanyProjectMemoFromManagement(db, companyProjectId, memo, now);

                db.SaveChanges();

                transaction.Commit();

                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}
