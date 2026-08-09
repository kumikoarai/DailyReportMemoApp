using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class CompanyProject
    {
        public int CompanyProjectId { get; set; }

        public int CompanyId { get; set; }

        public int ProjectId { get; set; }

        public string? Memo { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Company? Companies { get; set; }

        public Project? Projects { get; set; }

        public ICollection<ProjectTaskItem> ProjectTaskItems { get; set; } = new List<ProjectTaskItem>();
    }
}
