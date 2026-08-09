using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DailyReportMemoApp.ViewModels
{
    public class ChangeWorkTimeRange
    {
        public String SubTargetStartOrEnd { get; set; } = null!;

        public String SuccessOrFailure { get; set; } = null!;

        public int WorkTimeRangeId { get; set; } = -1!;

        public TextBox? TextBox { get; set; } = null!;
    }
}
