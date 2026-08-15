using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.ViewModels
{
    public class YearMonthItem
    {
        public int Year { get; set; }

        public int Month { get; set; }
    }

    public class CompanyProjectItem
    {
        public int CompanyId { get; set; }

        public int ProjectId { get; set; }

        public string? CompanyName { get; set; }

        public string? ProjectName { get; set; }
    }

    public class WorkLogGroup
    {
        public DateOnly WorkDate { get; set; }

        public int? SpecialTaskId { get; set; }

        public string? SpecialTaskName { get; set; }

        public int? CompanyProjectId { get; set; }

        public string? CompanyProjectName { get; set; }

        public List<WorkLogListGroup> WorkLogListGroups { get; set; } = new();
    }

    public class WorkLogListGroup
    {
        public int? ProjectTaskItemId { get; set; }

        public string? ProjectTaskItemName { get; set; }

        public List<WorkLog> WorkLogs { get; set; } = new();
    }




    public class WorkLogGroup2
    {
        public DateOnly WorkDate { get; set; }

        public int? SpecialTaskId { get; set; }

        public string? SpecialTaskName { get; set; }

        public int? CompanyProjectId { get; set; }

        public string? CompanyProjectName { get; set; }

        public List<WorkLogListGroup2> WorkLogListGroups2 { get; set; } = new();
    }

    public class WorkLogListGroup2
    {
        public int? ProjectTaskItemId { get; set; }

        public string? ProjectTaskItemName { get; set; }

        public List<WorkLogGroup3> WorkLogGroups3 { get; set; } = new();
    }


    public class WorkLogGroup3
    {
        public DateOnly WorkDate { get; set; }
        public List<WorkLog> WorkLogs { get; set; } = new();
    }

}
