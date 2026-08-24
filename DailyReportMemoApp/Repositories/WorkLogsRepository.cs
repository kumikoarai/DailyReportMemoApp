using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.ViewModels;
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
        /// 過去作業ログを年月で取得するメソッド
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <returns></returns>
        public List<WorkLogGroup> GetLogsByYearMonth(int year, int month)
        {
            using var db = new AppDbContext();

            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var workLogs = db.WorkLogs
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
                .Include(x => x.WorkingOnLogs)
                .Where(x =>
                    x.WorkingOnLogs != null &&
                    x.WorkingOnLogs.WorkDate >= startDate &&
                    x.WorkingOnLogs.WorkDate < endDate &&
                    x.WorkingOnLogs.WorkingOnFlg == false)
                .ToList();

            return workLogs
                .GroupBy(x => new
                {
                    WorkDate = x.WorkingOnLogs!.WorkDate,
                    SpecialTaskId = x.SpecialTaskId,
                    CompanyProjectId = x.ProjectTaskItems?.CompanyProjectId
                })
                .Select(g => new WorkLogGroup
                {
                    WorkDate = g.Key.WorkDate,

                    SpecialTaskId = g.Key.SpecialTaskId,

                    SpecialTaskName = g
                        .Select(x => x.SpecialTasks?.SpecialTaskName)
                        .FirstOrDefault(),

                    CompanyProjectId = g.Key.CompanyProjectId,

                    CompanyProjectName = g
                        .Where(x => x.ProjectTaskItems?.CompanyProjects != null)
                        .Select(x =>
                            x.ProjectTaskItems!.CompanyProjects!.Companies!.CompanyName
                            + " / " +
                            x.ProjectTaskItems.CompanyProjects.Projects!.ProjectName)
                        .FirstOrDefault(),

                    WorkLogListGroups = g
                        .OrderBy(x => x.WorkLogId)
                        .GroupBy(x => x.ProjectTaskItems?.TaskItemId)
                        .Select(gg => new WorkLogListGroup
                        {
                            ProjectTaskItemId = gg.Key,

                            ProjectTaskItemName = gg
                                .Select(x => x.ProjectTaskItems?.TaskItems?.TaskItemName)
                                .FirstOrDefault(),

                            WorkLogs = gg.ToList()
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.WorkDate)
                .ThenBy(x => x.SpecialTaskId == null)
                .ThenBy(x => x.SpecialTaskId)
                .ThenBy(x => x.CompanyProjectId)
                .ToList();
        }


        public List<WorkLogGroup2> GetLogsByCompanyProject(int companyId, int projectId)
        {
            using var db = new AppDbContext();

            var workLogs = db.WorkLogs
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
                .Include(x => x.WorkingOnLogs)
                .Where(x =>
                    x.ProjectTaskItems != null &&
                    x.ProjectTaskItems.CompanyProjects!.CompanyId == companyId &&
                    x.ProjectTaskItems.CompanyProjects!.ProjectId == projectId &&
                    x.WorkingOnLogs!.WorkingOnFlg == false)
                .OrderByDescending(x => x.WorkingOnLogs!.WorkDate)
                .ToList();

            return workLogs
                    .GroupBy(x => new
                    {
                        CompanyProjectId = x.ProjectTaskItems?.CompanyProjectId
                    })
                    .Select(g => new WorkLogGroup2
                    {
                        SpecialTaskName = g
                            .Select(x => x.SpecialTasks?.SpecialTaskName)
                            .FirstOrDefault(),

                        CompanyProjectId = g.Key.CompanyProjectId,

                        CompanyProjectName = g
                            .Where(x => x.ProjectTaskItems?.CompanyProjects != null)
                            .Select(x =>
                                x.ProjectTaskItems!.CompanyProjects!.Companies!.CompanyName
                                + " / " +
                                x.ProjectTaskItems.CompanyProjects.Projects!.ProjectName)
                            .FirstOrDefault(),

                        WorkLogListGroups2 = g
                            .OrderBy(x => x.WorkLogId)
                            .GroupBy(x => x.ProjectTaskItems?.TaskItemId)
                            .Select(gg => new WorkLogListGroup2
                            {
                                ProjectTaskItemId = gg.Key,

                                ProjectTaskItemName = gg
                                    .Select(x => x.ProjectTaskItems?.TaskItems?.TaskItemName)
                                    .FirstOrDefault(),

                                WorkLogGroups3 = gg
                                    .GroupBy(x => x.WorkingOnLogs!.WorkDate)
                                    .Select(ggg => new WorkLogGroup3
                                    {
                                        WorkDate = ggg.Key,
                                        WorkLogs = ggg.ToList()
                                    })
                                    .OrderByDescending(x => x.WorkDate)
                                    .ToList()
                            })
                            .OrderBy(x => x.ProjectTaskItemId)
                            .ToList()
                    })
                    .OrderByDescending(x => x.CompanyProjectId)
                    .ToList();

        }



        /// <summary>
        /// 作業ログを追加するメソッド
        /// </summary>
        /// <param name="workLog"></param>
        /// <returns></returns>
        public WorkLog AddWorkLog(AppDbContext db, WorkLog workLog)
        {
            db.WorkLogs.Add(workLog);
            db.SaveChanges();
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
