using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class WorkLog
    {
        public int WorkLogId { get; set; }

        public int? WorkingOnId { get; set; }

        public int? ProjectTaskItemId { get; set; }

        public int? SpecialTaskId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ProjectTaskItem? ProjectTaskItems { get; set; }

        public SpecialTask? SpecialTasks { get; set; }

        public WorkingOn? WorkingOnLogs { get; set; }

        public ICollection<WorkTimeRange> WorkTimeRanges { get; set; } = new List<WorkTimeRange>();
    }
}
