using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class SpecialTask
    {
        public int SpecialTaskId { get; set; }

        public string SpecialTaskName { get; set; } = String.Empty;

        public bool DefaultStartFlg { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public bool IsActive { get; set; } = false;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<WorkLog> WorkLogs { get; set; } = new List<WorkLog>();
    }
}
