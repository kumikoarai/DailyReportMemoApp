using DailyReportMemoApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyReportMemoApp.Repositories
{
    public class WorkLogsRepository
    {
        /// <summary>
        /// 本日の作業ログを取得するメソッド
        /// </summary>
        /// <param name="workingOn"></param>
        /// <returns></returns>
        public List<WorkLog> GetWorkLogs(int workingOnId)
        {
            using (var db = new Data.AppDbContext())
            {
                return db.WorkLogs
                    .AsNoTracking()
                    .AsSplitQuery()
                    .Include(x => x.WorkTimeRanges)
                    .Include(x => x.ProjectTaskItems)
                        .ThenInclude(x => x.TaskItems)
                    .Include(x => x.ProjectTaskItems)
                        .ThenInclude(x => x.CompanyProjects)
                            .ThenInclude(x => x.Companies)
                    .Include(x => x.ProjectTaskItems)
                        .ThenInclude(x => x.CompanyProjects)
                            .ThenInclude(x => x.Projects)
                   .Include(x => x.SpecialTasks)
                   .Where(wl => wl.WorkingOnId == workingOnId)
                   .OrderBy(wl => wl.SpecialTaskId == null)
                   .ThenBy(wl => wl.SpecialTaskId)
                   .ThenBy(wl => wl.ProjectTaskItems!.CompanyProjects!.CompanyId)
                   .ThenBy(wl => wl.ProjectTaskItems!.CompanyProjects!.ProjectId)
                   .ThenBy(wl => wl.ProjectTaskItems!.TaskItemId)
                   .ToList();
            }
        }


        /// <summary>
        /// 簡易的な作業ログを取得するメソッド
        /// </summary>
        /// <param name="workingOnId"></param>
        /// <returns></returns>
        public List<WorkLog> GetSimpleWorkLogs(int workingOnId)
        {
            using (var db = new Data.AppDbContext())
            {
                return db.WorkLogs
                   .Include(x => x.WorkTimeRanges)
                   .Where(wl => wl.WorkingOnId == workingOnId)
                   .ToList();
            }
        }


        /// <summary>
        /// 作業ログを追加するメソッド
        /// </summary>
        /// <param name="workLog"></param>
        /// <returns></returns>
        public WorkLog AddWorkLog(WorkLog workLog)
        {
            using (var db = new Data.AppDbContext())
            {
                db.WorkLogs.Add(workLog);
                db.SaveChanges();
            }
            return workLog;
        }

        /// <summary>
        /// 案件タスク項目が存在するかどうかを確認するメソッド
        /// </summary>
        /// <param name="projectTaskItemId"></param>
        /// <returns></returns>
        public bool ProjectTaskItemExists(int workingOnLogsID, int projectTaskItemId)
        {
            using (var db = new Data.AppDbContext())
            {
                return db.WorkLogs.Any(pti => pti.ProjectTaskItemId == projectTaskItemId && pti.WorkingOnId == workingOnLogsID);
            }
        }

    }
}
