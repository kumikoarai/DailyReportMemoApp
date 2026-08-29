using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.Utils;
using DailyReportMemoApp.ViewModels;
using Microsoft.EntityFrameworkCore;
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
    /// ProjectManagementWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectManagementWindow : Window
    {
        private CompanyProjectsRepository _companyProjectsRepository = new();
        private CompaniesRepository _companiesRepository = new();

        public ProjectManagementWindow()
        {
            InitializeComponent();

            LoadButtons();
        }


        /// <summary>
        /// ボタンを読み込み、企業・案件ごとにグループ化して表示するメソッド
        /// </summary>
        private void LoadButtons()
        {
            BtnListPanel.Children.Clear();

            //企業・案件のラベルを追加-------------------------------------------
            var CompaniesProjectsTTL = CreateTTLTextBoxBlock("会社一覧");
            BtnListPanel.Children.Add(CompaniesProjectsTTL);

            //企業・案件ごとにExpanderを作成し、案件ボタンを追加---------------------
            List<Company> allCompanies = _companiesRepository.GetAllCompanies();
            if (allCompanies == null)
            {
                return;
            }

            foreach (var company in allCompanies)
            {
                var companyPanel = new StackPanel();

                var companyButton = new Button
                {
                    Content = new TextBlock
                    {
                        Text = $"{company.CompanyName}",
                        TextWrapping = TextWrapping.Wrap
                    },
                    Tag = company,
                };

                companyButton.Click += CompanyButton_Click;
                companyPanel.Children.Add(companyButton);

                BtnListPanel.Children.Add(companyPanel);
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
        /// 会社ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompanyButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button companyButton)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (companyButton.Tag is not Company company)
            {
                return;
            }

            ContentFrame.Navigate(new ProjectManagementPage(company));
        }

        /// <summary>
        /// 閉じるボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseProjectManagement_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        /// <summary>
        /// 会社名を入力して追加するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CompanyEntry_Click(object sender, RoutedEventArgs e)
        {
            var companyName = CompanyName.Text.Trim();

            if (string.IsNullOrWhiteSpace(companyName))
            {
                MessageBox.Show("会社名を入力してください。");
                return;
            }

            try
            {
                var company = new Company
                {
                    CompanyName = companyName,
                    CreatedAt = DateTime.Now
                };
                _companiesRepository.AddCompany(company);

                LoadButtons();
                CompanyName.Text = "";
            }
            catch (DbUpdateException ex)
            {
                ErrorLogger.Write(ex);
                MessageBox.Show(
                    "会社データの保存に失敗しました。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

    }
}
