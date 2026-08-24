using DailyReportMemoApp.Data;
using DailyReportMemoApp.Utils;
using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Threading;

namespace DailyReportMemoApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            DispatcherUnhandledException +=
               App_DispatcherUnhandledException;

            base.OnStartup(e);

            using (var db = new AppDbContext())
            {
                db.Database.Migrate();
            }
        }

        private void App_DispatcherUnhandledException(
            object sender,
            DispatcherUnhandledExceptionEventArgs e)
        {
            ErrorLogger.Write(e.Exception);

            MessageBox.Show(
                "予期しないエラーが発生しました。\n" +
                "エラーログを記録しました。",
                "エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            e.Handled = true;
        }
    }

}
