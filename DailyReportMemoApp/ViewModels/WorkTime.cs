using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DailyReportMemoApp.ViewModels
{
    public class WorkTime
    {
        public String StartOrEnd { get; set; } = null!;

        public WorkTimeRange WorkTimeRange { get; set; } = null!;

        public bool Sagyochu { get; set; } = false!;
    }
}
