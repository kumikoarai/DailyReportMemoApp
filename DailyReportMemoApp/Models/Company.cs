using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Models
{
    public class Company
    {
        public int CompanyId { get; set; }

        public string CompanyName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<CompanyProject> CompanyProjects { get; set; } = new List<CompanyProject>();
    }
}
