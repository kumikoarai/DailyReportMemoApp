using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class TaskItem
    {
        public int TaskItemId { get; set; }

        public string TaskItemName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProjectTaskItem> ProjectTaskItems { get; set; } = new List<ProjectTaskItem>();
    }
}
