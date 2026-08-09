using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class WorkTimeRange
    {
        public int WorkTimeRangeId { get; set; }

        public int WorkLogId { get; set; }

        public TimeSpan? StartTime { get; set; }

        public TimeSpan? EndTime { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public WorkLog WorkLogs { get; set; } = null!;

        [NotMapped]
        public TimeSpan? Duration => EndTime - StartTime;
    }
}
