using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class ProjectTaskItemsRepository
    {
        /// <summary>
        /// 案件タスクのリストを取得するメソッド
        /// </summary>
        /// <param name="companyProjectId"></param>
        /// <returns></returns>
        public List<ProjectTaskItem> GetProjectTaskItems(int companyProjectId)
        {
            using (var db = new AppDbContext())
            {
                return db.ProjectTaskItems
                       .Include(x => x.CompanyProjects)
                            .ThenInclude(x => x.Companies)
                       .Include(x => x.CompanyProjects)
                            .ThenInclude(x => x.Projects)
                       .Include(x => x.TaskItems)
                       .Where(cp => cp.CompanyProjectId == companyProjectId)
                       .ToList();
            };
        }



        /// <summary>
        /// 使用中の案件タスクのリストを取得するメソッド
        /// </summary>
        /// <param name="companyProjectId"></param>
        /// <returns></returns>
        public List<ProjectTaskItem> GetCurrentProjectTaskItems()
        {
            using (var db = new AppDbContext())
            {
                return db.ProjectTaskItems
                       .Include(x => x.CompanyProjects)
                            .ThenInclude(x => x.Companies)
                       .Include(x => x.CompanyProjects)
                            .ThenInclude(x => x.Projects)
                       .Include(x => x.TaskItems)
                       .Where(cp => cp.IsCurrent)
                       .ToList();
            };
        }

        /// <summary>
        /// 案件タスクを追加するメソッド
        /// </summary>
        /// <param name="projectTaskItem"></param>
        /// <returns></returns>
        public ProjectTaskItem AddProjectTaskItem(AppDbContext db, ProjectTaskItem projectTaskItem)
        {
            db.ProjectTaskItems.Add(projectTaskItem);
            db.SaveChanges();

            return projectTaskItem;
        }

        /// <summary>
        /// 案件作業項目の本日対応フラグを更新
        /// </summary>
        /// <param name="ProjectTaskItemId"></param>
        /// <returns></returns>
        public bool UpdateProjectTaskItemIsCurrent(AppDbContext db, int? ProjectTaskItemId)
        {
            var existingProjectTaskItem = db.ProjectTaskItems.Find(ProjectTaskItemId);
            if (existingProjectTaskItem == null)
            {
                return false;
            }

            var now = DateTime.Now;
            existingProjectTaskItem.IsCurrent = true;
            existingProjectTaskItem.UpdatedAt = now;
            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// 案件作業項目の全ての本日対応フラグをfalseに更新
        /// </summary>
        /// <returns></returns>
        public bool UpdateProjectTaskItemIsNotCurrent(AppDbContext db)
        {
            var curProjectTaskItems = db.ProjectTaskItems
                                        .Where(cp => cp.IsCurrent)
                                        .ToList();

            foreach (var curProjectTaskItem in curProjectTaskItems)
            {
                curProjectTaskItem.IsCurrent = false;
                curProjectTaskItem.UpdatedAt = DateTime.Now;
                db.SaveChanges();
            }

            return true;
        }
    }
}
