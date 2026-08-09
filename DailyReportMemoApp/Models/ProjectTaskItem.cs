using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class ProjectTaskItem
    {
        public int ProjectTaskItemId { get; set; }

        public int CompanyProjectId { get; set; }

        public int TaskItemId { get; set; }

        public bool IsCurrent { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public CompanyProject? CompanyProjects { get; set; }

        public TaskItem? TaskItems { get; set; }

        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
