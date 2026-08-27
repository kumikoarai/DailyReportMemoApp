using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.ViewModels
{
    public class Common
    {
        /// <summary>
        /// TimeSpanを取得するメソッド
        /// </summary>
        /// <param name="now"></param>
        /// <returns></returns>
        public static TimeSpan workTimeSpan(DateTime now, WorkingOn? workingOnLogs)
        {
            DateOnly today = DateOnly.FromDateTime(now);
            int hour = now.Hour;

            if (workingOnLogs == null)
            {
                return new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }

            if (workingOnLogs.WorkDate < today)
            {
                int days = today.DayNumber - workingOnLogs.WorkDate.DayNumber;

                hour = now.Hour + (days * 24);
            }

            return new TimeSpan(hour, DateTime.Now.Minute, DateTime.Now.Second);

        }

        /// <summary>
        /// timeSpanに変換
        /// </summary>
        /// <param name="value"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public bool TryParseTime(string value, out TimeSpan time)
        {
            time = default;

            var parts = value.Split(':');

            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int hours) ||
                !int.TryParse(parts[1], out int minutes) ||
                hours < 0 ||
                minutes < 0 || minutes >= 60)
            {
                return false;
            }

            time = TimeSpan.FromHours(hours)
                 + TimeSpan.FromMinutes(minutes);

            return true;
        }

        /// <summary>
        /// TimeSpanから秒を削除するメソッド
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public TimeSpan RemoveSeconds(TimeSpan time) 
        {
            return new TimeSpan(
                time.Days,
                time.Hours,
                time.Minutes,
                0
            );
        }
    }
}
