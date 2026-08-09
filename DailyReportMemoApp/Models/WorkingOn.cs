using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class WorkingOn
    {
        public int WorkingOnId { get; set; }

        public bool WorkingOnFlg { get; set; } = false;

        public DateOnly WorkDate { get; set; }

        public DateTime WorkingOnStart { get; set; }

        public DateTime? WorkingOnEnd { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
