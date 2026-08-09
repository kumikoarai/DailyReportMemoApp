using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public bool Completed { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<CompanyProject> CompanyProjects { get; set; } = new List<CompanyProject>();
    }
}
