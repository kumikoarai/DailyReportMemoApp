using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.ViewModels;
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
    /// PastWorkLogsWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class PastWorkLogsWindow : Window
    {
        private WorkingOnRepository _workingOnRepository = new();
        private CompanyProjectsRepository _companyProjectsRepository = new();

        public PastWorkLogsWindow()
        {
            InitializeComponent();

            LoadButtons();
        }

        /// <summary>
        /// ボタンを読み込み、年月や企業・案件ごとにグループ化して表示するメソッド
        /// </summary>
        private void LoadButtons() 
        {
            BtnListPanel.Children.Clear();

            //年月のラベルを追加-------------------------------------------
            var yearMonthTTL = CreateTTLTextBoxBlock("年月");
            BtnListPanel.Children.Add(yearMonthTTL);

            //年ごとにExpanderを作成し、月ボタンを追加---------------------
            List<YearMonthItem> yearMonthItems = _workingOnRepository.GetWorkYearMonth();

            if (yearMonthItems == null)
            {
                return;
            }
            var groupedYearMonths = yearMonthItems
                                        .GroupBy(x => x.Year);

            foreach (var yearGroup in groupedYearMonths)
            {
                var yearExpander = new Expander
                {
                    Header = $"{yearGroup.Key}年",
                    Margin = new Thickness(0, 0, 0, 20)
                };

                var monthPanel = new StackPanel();

                foreach (var yearMonth in yearGroup)
                {
                    var monthButton = new Button
                    {
                        Content = $"{yearMonth.Month}月",
                        Tag = new YearMonthItem
                        {
                            Year = yearMonth.Year,
                            Month = yearMonth.Month
                        }
                    };

                    monthButton.Click += MonthButton_Click;
                    monthPanel.Children.Add(monthButton);
                }

                yearExpander.Content = monthPanel;
                BtnListPanel.Children.Add(yearExpander);
            }


            //企業・案件のラベルを追加-------------------------------------------
            var CompaniesProjectsTTL = CreateTTLTextBoxBlock("企業・案件");
            BtnListPanel.Children.Add(CompaniesProjectsTTL);

            //企業・案件ごとにExpanderを作成し、案件ボタンを追加---------------------
            List<CompanyProjectItem> companyProjectItems = _companyProjectsRepository.GetCompanyProjectsDistinct();
            if (companyProjectItems == null)
            {
                return;
            }
            var groupedCompanyProjectItems = companyProjectItems
                                        .GroupBy(x => x.CompanyName);

            foreach (var CompanyGroup in groupedCompanyProjectItems)
            {
                var CompanyExpander = new Expander
                {
                    Header = $"{CompanyGroup.Key}",
                    Margin = new Thickness(0, 0, 0, 20)
                };

                var projectPanel = new StackPanel();

                foreach (var companyProject in CompanyGroup)
                {
                    var projectButton = new Button
                    {
                        Content = new TextBlock
                        {
                            Text = $"{companyProject.ProjectName}",
                            TextWrapping = TextWrapping.Wrap
                        },
                        Tag = new CompanyProjectItem
                        {
                            CompanyId = companyProject.CompanyId,
                            ProjectId = companyProject.ProjectId,
                            CompanyName = companyProject.CompanyName,
                            ProjectName = companyProject.ProjectName
                        },
                    };

                    projectButton.Click += ProjectButton_Click;
                    projectPanel.Children.Add(projectButton);
                }

                CompanyExpander.Content = projectPanel;
                BtnListPanel.Children.Add(CompanyExpander);
            }
        }

        /// <summary>
        /// タイトル用のテキストボックスを作成するメソッド
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private Border CreateTTLTextBoxBlock(string text)
        {
            var border = new Border
            {
                Background = Brushes.AliceBlue,
                Padding = new Thickness(3),
                CornerRadius = new CornerRadius(4),
                Margin = new Thickness(0, 0, 0, 3)
            };

            var textPanelTtl = new StackPanel
            {
                Margin = new Thickness(0, 2, 0, 2)
            };

            textPanelTtl.Children.Add(new TextBox
            {
                Text = text,
                IsReadOnly = true,
                Margin = new Thickness(0),
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent,
                IsReadOnlyCaretVisible = false
            });

            border.Child = textPanelTtl;

            return border;
        }

        /// <summary>
        /// 月ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button monthButton)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (monthButton.Tag is not YearMonthItem yearMonthItem)
            {
                return;
            }

            ContentFrame.Navigate(new PastWorkLogsPage(yearMonthItem, null));
        }

        /// <summary>
        /// 案件ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button projectButton)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (projectButton.Tag is not CompanyProjectItem companyProjectItem)
            {
                return;
            }

            ContentFrame.Navigate(new PastWorkLogsPage(null,companyProjectItem));
        }

    }
}
