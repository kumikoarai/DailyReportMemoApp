using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class WorkTimeRangesRepository
    {
        private Common _common = new();


        /// <summary>
        /// 作業時間範囲を取得するメソッド
        /// </summary>
        /// <param name="workLogIds"></param>
        /// <returns></returns>
        public List<WorkTimeRange> GetWorkTimeRangeOrderStartTime(int[] workLogIds)
        {
            using (var db = new Data.AppDbContext()) 
            {
                return db.WorkTimeRanges
                        .Where(wtr => workLogIds.Contains(wtr.WorkLogId))
                        .AsEnumerable()
                        .OrderBy(wtr => wtr.StartTime)
                        .ToList();
            }
        }

        /// <summary>
        /// 作業時間範囲を追加するメソッド
        /// </summary>
        /// <param name="workTimeRange"></param>
        /// <returns></returns>
        public WorkTimeRange AddWorkTimeRange(AppDbContext db, WorkTimeRange workTimeRange)
        { 
            db.WorkTimeRanges.Add(workTimeRange);
            db.SaveChanges();
            return workTimeRange;
        }

        /// <summary>
        /// 作業時間範囲を終了するメソッド
        /// </summary>
        /// <param name="workTimeRange"></param>
        /// <returns></returns>
        public bool UpdateWorkTimeRangeEndTime(AppDbContext db, WorkTimeRange workTimeRange, DateTime now, TimeSpan timeSpan)
        {
            var existingWorkTimeRange = db.WorkTimeRanges.Find(workTimeRange.WorkTimeRangeId);
            if (existingWorkTimeRange == null)
            {
                return false;
            }

            existingWorkTimeRange.EndTime = timeSpan;
            existingWorkTimeRange.UpdatedAt = now;
            db.SaveChanges();
            return true;
        }

        /// <summary>
        /// 作業時間のレコードを削除する
        /// </summary>
        /// <param name="workTimeRangeId"></param>
        /// <returns></returns>
        public bool DeleteWorkTimeRange(AppDbContext db, int workTimeRangeId) 
        {
            var workTimeRange = db.WorkTimeRanges.FirstOrDefault(x => x.WorkTimeRangeId == workTimeRangeId);

            if (workTimeRange != null) 
            {
                db.WorkTimeRanges.Remove(workTimeRange);
                db.SaveChanges();

                return true;
            }
            return false;
        }

        /// <summary>
        /// 作業時間範囲の終了時間をnullに書き換える
        /// </summary>
        /// <param name="workTimeRange"></param>
        /// <param name="workingOnId"></param>
        /// <returns></returns>
        public bool UpdateWorkTimeRangeTimeEndTimeDel(AppDbContext db, WorkTimeRange workTimeRange, int workingOnId) 
        {
            var targetWorkTimeRange = db.WorkTimeRanges
                                .Include(wtr => wtr.WorkLogs)
                                .FirstOrDefault(wtr =>
                                    wtr.WorkLogs != null &&
                                    wtr.WorkLogs.WorkingOnId == workingOnId &&
                                    wtr.EndTime == workTimeRange.StartTime);

            if (targetWorkTimeRange == null)
            {
                return false;
            }

            targetWorkTimeRange.EndTime = null;
            targetWorkTimeRange.UpdatedAt = DateTime.Now;
            db.SaveChanges();

            return true;
        }

        /// <summary>
        /// 作業時間範囲の開始時間または終了時間を更新するメソッド
        /// </summary>
        /// <param name="workTime"></param>
        /// <param name="newTime"></param>
        /// <param name="workingOnId"></param>
        /// <returns></returns>
        public ChangeWorkTimeRange UpdateWorkTimeRangeTime(AppDbContext db, WorkTime workTime, String newTime, int workingOnId)
        {
            // newTimeをTimeSpanに変換
            if (!_common.TryParseTime(newTime, out TimeSpan parsedNewTime))
            {
                var changeWorkTimeRange = new ChangeWorkTimeRange
                {
                    SuccessOrFailure = "f",
                    WorkTimeRangeId = workTime.WorkTimeRange.WorkTimeRangeId
                };

                return changeWorkTimeRange; // 変換に失敗した場合はIDを返す
            }

            var changeFailureWorkTimeRange = new ChangeWorkTimeRange
            {
                SuccessOrFailure = "f",
                WorkTimeRangeId = -1
            };

            var changeSuccessWorkTimeRange = new ChangeWorkTimeRange
            {
                SuccessOrFailure = "s",
                WorkTimeRangeId = -1
            };


            // 作業時間範囲の開始時間または終了時間を更新する処理
            if (workTime.StartOrEnd == "start") {

                // 入力された開始時間とおなじ時間を持つ終了時間のレコードの開始時間よりも、入力された新しい開始時間が小さい場合は、更新しない
                var targetEndWorkTimeRange = db.WorkTimeRanges
                    .Include(wtr => wtr.WorkLogs)
                    .FirstOrDefault(wtr =>
                        wtr.WorkLogs != null &&
                        wtr.WorkLogs.WorkingOnId == workingOnId &&
                        wtr.EndTime == workTime.WorkTimeRange.StartTime);
                if (targetEndWorkTimeRange != null && parsedNewTime < targetEndWorkTimeRange.StartTime)
                {
                    changeFailureWorkTimeRange.SubTargetStartOrEnd = "end";
                    changeFailureWorkTimeRange.WorkTimeRangeId = targetEndWorkTimeRange.WorkTimeRangeId;

                    return changeFailureWorkTimeRange; // 更新しない
                }

                // 作業時間範囲の開始時間を更新する場合
                var targetStartWorkTimeRange = db.WorkTimeRanges
                    .Include(wtr => wtr.WorkLogs)
                    .FirstOrDefault(wtr =>
                        wtr.WorkLogs != null &&
                        wtr.WorkTimeRangeId == workTime.WorkTimeRange.WorkTimeRangeId);

                if (targetStartWorkTimeRange == null)
                {

                    return changeFailureWorkTimeRange;
                }

                targetStartWorkTimeRange.StartTime = parsedNewTime;
                targetStartWorkTimeRange.UpdatedAt = DateTime.Now;


                if (targetEndWorkTimeRange == null)
                {
                    return changeFailureWorkTimeRange;
                }

                targetEndWorkTimeRange.EndTime = parsedNewTime;
                targetEndWorkTimeRange.UpdatedAt = DateTime.Now;

                changeSuccessWorkTimeRange.SubTargetStartOrEnd = "end";
                changeSuccessWorkTimeRange.WorkTimeRangeId = targetEndWorkTimeRange.WorkTimeRangeId;


            }
            else if (workTime.StartOrEnd == "end") {
                // 入力された終了時間とおなじ時間を持つ開始時間のレコードの終了時間よりも、入力された新しい終了時間が大きい場合は、更新しない
                var targetStartWorkTimeRange = db.WorkTimeRanges
                    .Include(wtr => wtr.WorkLogs)
                    .FirstOrDefault(wtr =>
                        wtr.WorkLogs != null &&
                        wtr.WorkLogs.WorkingOnId == workingOnId &&
                        wtr.StartTime == workTime.WorkTimeRange.EndTime);
                if (targetStartWorkTimeRange != null && parsedNewTime > targetStartWorkTimeRange.EndTime) 
                {
                    changeFailureWorkTimeRange.SubTargetStartOrEnd = "start";
                    changeFailureWorkTimeRange.WorkTimeRangeId = targetStartWorkTimeRange.WorkTimeRangeId;

                    return changeFailureWorkTimeRange; // 更新しない
                }

                // 作業時間範囲の終了時間を更新する処理
                var targetEndWorkTimeRange = db.WorkTimeRanges
                    .Include(wtr => wtr.WorkLogs)
                    .FirstOrDefault(wtr =>
                        wtr.WorkLogs != null &&
                        wtr.WorkTimeRangeId == workTime.WorkTimeRange.WorkTimeRangeId);
                if (targetEndWorkTimeRange == null)
                {
                    return changeFailureWorkTimeRange;
                }
                targetEndWorkTimeRange.EndTime = parsedNewTime;
                targetEndWorkTimeRange.UpdatedAt = DateTime.Now;

                if (targetStartWorkTimeRange == null)
                {
                    return changeFailureWorkTimeRange;
                }

                targetStartWorkTimeRange.StartTime = parsedNewTime;
                targetStartWorkTimeRange.UpdatedAt = DateTime.Now;

                changeSuccessWorkTimeRange.SubTargetStartOrEnd = "start";
                changeSuccessWorkTimeRange.WorkTimeRangeId = targetStartWorkTimeRange.WorkTimeRangeId;
            }
            else
            {
                return changeFailureWorkTimeRange; // "start"でも"end"でもない場合は-1を返す
            }

            db.SaveChanges();

            return changeSuccessWorkTimeRange;
        }
    }
}
