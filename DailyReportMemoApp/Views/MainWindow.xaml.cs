using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.Views;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyReportMemoApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WorkingOn? _workingOnLogs = new ();
        private WorkingOnRepository _workingOnRepository = new();

        public MainWindow()
        {
            InitializeComponent();

            TodayDate.Text = DateTime.Today.ToString("yyyy年MM月dd日");

            // 作業中のタスクが存在するかどうかをチェックする
            _workingOnLogs = _workingOnRepository.GetWorkingOnLogs();
            if (_workingOnLogs != null)
            {
                var now = DateTime.Now;
                if (_workingOnLogs.WorkDate == DateOnly.FromDateTime(now))
                {
                    MessageBox.Show("作業中のタスクが存在します。作業中のタスクを終了してください。", "作業中のタスクが存在します", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show("作業中のタスクが存在しますが、作業日が本日ではありません。作業中のタスクを終了してください。", "作業中のタスクが存在します", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                WorkEntryWindow workEntryWindow = new();
                workEntryWindow.ShowDialog();
            }

        }

        private void OpenWorkEntryWindow_Click(object sender, RoutedEventArgs e)
        {

            WorkEntryWindow workEntryWindow = new()
            {
                Owner = this
            };

            Hide();

            workEntryWindow.ShowDialog();

            Show();
            Activate();

        }

        private void OpenSpecialTasksEntryWindow_Click(object sender, RoutedEventArgs e)
        {
            SpecialTasksEntryWindow specialTasksEntryWindow = new()
            {
                Owner = this
            };

            specialTasksEntryWindow.ShowDialog();

        }

        private void OpenPastWorkLogsWindow_Click(object sender, RoutedEventArgs e)
        {
            PastWorkLogsWindow pastWorkLogsWindow = new()
            {
                Owner = this
            };

            pastWorkLogsWindow.ShowDialog();

        }

        private void OpenProjectManagementWindow_Click(object sender, RoutedEventArgs e)
        {
            ProjectManagementWindow projectManagementWindow = new()
            {
                Owner = this
            };

            projectManagementWindow.ShowDialog();

        }

        private void OpenHelpWindow_Click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new()
            {
                Owner = this
            };

            helpWindow.ShowDialog();

        }
    }
}