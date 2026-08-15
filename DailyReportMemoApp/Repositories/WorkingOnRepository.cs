using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class WorkingOnRepository
    {
        /// <summary>
        /// 作業中のタスクが存在するかどうかをチェックするメソッド
        /// </summary>
        /// <returns>作業中のログを返す</returns>
        public WorkingOn? GetWorkingOnLogs()
        {
            using (var db = new AppDbContext())
            {
                return db.WorkingOnLogs
                       .FirstOrDefault(x => x.WorkingOnFlg);
            };

        }

        /// <summary>
        /// 過去作業ログのすべての年月種類を取得する
        /// </summary>
        /// <returns></returns>
        public List<YearMonthItem> GetWorkYearMonth() 
        {
            using (var db = new AppDbContext())
            {
                return db.WorkingOnLogs
                            .Select(x => new
                            {
                                Year = x.WorkDate.Year,
                                Month = x.WorkDate.Month
                            })
                            .Distinct()
                            .OrderByDescending(x => x.Year)
                            .ThenByDescending(x => x.Month)
                            .Select(x => new YearMonthItem
                            {
                                Year = x.Year,
                                Month = x.Month
                            })
                            .ToList();
            }
        }

        /// <summary>
        /// 本日の作業ログを追加するメソッド
        /// </summary>
        /// <param name="workingOn"></param>
        /// <returns></returns>
        public WorkingOn AddWorkingOn(WorkingOn workingOn)
        {
            using (var db = new Data.AppDbContext())
            {
                db.WorkingOnLogs.Add(workingOn);
                db.SaveChanges();
            }
            return workingOn;
        }

        /// <summary>
        /// 作業中のタスクを終了するメソッド
        /// </summary>
        /// <param name="workingOn"></param>
        /// <returns></returns>
        public bool WorkingOnLogCompleted(WorkingOn workingOn)
        { 
            using (var db = new Data.AppDbContext())
            {
                var existingWorkingOnLog = db.WorkingOnLogs.Find(workingOn.WorkingOnId);
                if (existingWorkingOnLog == null)
                {
                    return false;
                }

                var now = DateTime.Now;
                existingWorkingOnLog.WorkingOnFlg = false;
                existingWorkingOnLog.WorkingOnEnd = now;
                existingWorkingOnLog.UpdatedAt = now;
                db.SaveChanges();
            }
            return true;
        }
    }
}
