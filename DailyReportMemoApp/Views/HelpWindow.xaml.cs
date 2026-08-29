using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
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
    /// HelpWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class HelpWindow : Window
    {
        private ScrollViewer? _currentHelpContent;

        public HelpWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ヘルプメニューボタンの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpMenuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (HelpMenuListBox.SelectedItem is not ListBoxItem selectedItem)
            {
                return;
            }

            // 現在表示しているヘルプを非表示
            if (_currentHelpContent != null)
            {
                _currentHelpContent.Visibility = Visibility.Collapsed;
            }

            //洗濯したヘルプメニューのタグを取得
            var tag = selectedItem.Tag?.ToString();

            if (tag == null)
            {
                return;
            }

            // 選択されたヘルプを取得
            if (FindName(tag) is ScrollViewer scrollViewer)
            {
                scrollViewer.Visibility = Visibility.Visible;
                _currentHelpContent = scrollViewer;
            }
        }
    }

}
