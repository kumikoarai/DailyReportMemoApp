using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DailyReportMemoApp.ViewModels
{
    public class MemoSaveInfo
    {
        public WorkLog WorkLog { get; set; } = null!;

        public TextBox TextBox { get; set; } = null!;
    }
}
