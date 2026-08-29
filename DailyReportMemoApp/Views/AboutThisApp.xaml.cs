using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DailyReportMemoApp.Views
{
    /// <summary>
    /// AboutThisApp.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutThisApp : Window
    {
        public AboutThisApp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 「GitHubを開く」ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGitHub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://github.com/kumikoarai",
                UseShellExecute = true
            });
        }
    }
}
