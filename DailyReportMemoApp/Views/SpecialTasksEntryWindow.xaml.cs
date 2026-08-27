using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.Utils;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DailyReportMemoApp
{
    /// <summary>
    /// SpecialTasksEntryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class SpecialTasksEntryWindow : Window
    {
        public SpecialTasksEntryWindow()
        {
            InitializeComponent();

            LoadSpecialTasks();

        }

        private void SpecialTaskEntry_Click(object sender, RoutedEventArgs e)
        {
            var specialTaskName = SpecialTaskName.Text.Trim();

            if (string.IsNullOrWhiteSpace(specialTaskName))
            {
                MessageBox.Show("作業項目名を入力してください。");
                return;
            }

            try
            {
                using (var db = new AppDbContext())
                {
                    var specialTask = new SpecialTask
                    {
                        SpecialTaskName = specialTaskName,
                        DefaultStartFlg = false,
                        CreatedAt = DateTime.Now
                    };

                    db.SpecialTasks.Add(specialTask);
                    db.SaveChanges();
                }

                LoadSpecialTasks();

                SpecialTaskName.Text = "";
            }
            catch (DbUpdateException ex)
            {
                ErrorLogger.Write(ex);

                MessageBox.Show(
                    "特別作業項目名データの保存に失敗しました。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }
        }

        /// <summary>
        /// Loads special tasks from the database and displays them in a grid with checkboxes and radio buttons for user interaction.
        /// デフォルトの作業項目の設定や使用中の作業項目の管理を行うために、データベースから特別な作業項目を読み込み、チェックボックスとラジオボタンを含むグリッドに表示します。
        /// </summary>
        private void LoadSpecialTasks()
        {
            SpecialTasksListPanel.Children.Clear();

            var itemsNoNExist = false;

            var grid = new Grid
            {
                Margin = new Thickness(5)
            };

            // 1列目：使用する項目のチェックボックス用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            // 1列目：デフォルトスタート用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            // 2列目：作業項目用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            // 3列目：削除ボタン用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            // 0行目をタイトル行として追加
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });

            // タイトルを作成
            var activeTitle = new TextBlock
            {
                Text = "使用中",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            var DefaultStartTitle = new TextBlock
            {
                Text = "初期開始",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            var specialTaskTitle = new TextBlock
            {
                Text = "作業項目",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            var deleteTitle = new TextBlock
            {
                Text = "",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            // タイトルの配置
            Grid.SetRow(activeTitle, 0);
            Grid.SetColumn(activeTitle, 0);

            Grid.SetRow(DefaultStartTitle, 0);
            Grid.SetColumn(DefaultStartTitle, 1);

            Grid.SetRow(specialTaskTitle, 0);
            Grid.SetColumn(specialTaskTitle, 2);

            Grid.SetRow(deleteTitle, 0);
            Grid.SetColumn(deleteTitle, 3);

            // タイトルをGridに追加する
            grid.Children.Add(activeTitle);
            grid.Children.Add(DefaultStartTitle);
            grid.Children.Add(specialTaskTitle);
            grid.Children.Add(deleteTitle);

            using (var db = new AppDbContext())
            {
                var specialTasks = db.SpecialTasks
                                     .Where(x => !x.IsDeleted)
                                     .ToList();

                if (specialTasks.Count <= 0)
                {
                    itemsNoNExist = true;
                }

                for (int i = 0; i < specialTasks.Count; i++)
                {
                    var specialTask = specialTasks[i];

                    // 0行目はタイトルなので、データ行は1行目から
                    int rowIndex = i + 1;


                    // 1件につき1行追加
                    grid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    var checkBox = new CheckBox
                    {
                        IsChecked = specialTask.IsActive,
                        Tag = specialTask.SpecialTaskId,
                        Margin = new Thickness(5),
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    checkBox.Checked += CheckBox_Changed;
                    checkBox.Unchecked += CheckBox_Changed;

                    var radioButton = new RadioButton
                    {
                        GroupName = "SpecialTaskGroup",
                        Margin = new Thickness(5),
                        Tag = specialTask.SpecialTaskId,
                        IsChecked = specialTask.DefaultStartFlg,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    radioButton.Checked += RadioButton_Checked;

                    var textBlock = new TextBlock
                    {
                        Text = $"{specialTask.SpecialTaskName}",
                        Margin = new Thickness(5),
                        VerticalAlignment = VerticalAlignment.Center,
                        Cursor = System.Windows.Input.Cursors.Hand,
                        TextWrapping = TextWrapping.Wrap
                    };

                    textBlock.MouseLeftButtonDown += (s, e) =>
                    {
                        radioButton.IsChecked = true;
                    };

                    var deleteButton = new Button
                    {
                        Content = "削除",
                        Margin = new Thickness(5),
                        Tag = specialTask.SpecialTaskId,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    deleteButton.Click += DeleteButton_Clicked;

                    Grid.SetRow(checkBox, rowIndex);
                    Grid.SetColumn(checkBox, 0);

                    Grid.SetRow(radioButton, rowIndex);
                    Grid.SetColumn(radioButton, 1);

                    Grid.SetRow(textBlock, rowIndex);
                    Grid.SetColumn(textBlock, 2);

                    Grid.SetRow(deleteButton, rowIndex);
                    Grid.SetColumn(deleteButton, 3);

                    grid.Children.Add(checkBox);
                    grid.Children.Add(radioButton);
                    grid.Children.Add(textBlock);
                    grid.Children.Add(deleteButton);

                }
            }

            //作業項目が存在しない場合
            if(itemsNoNExist)
            {
                var itemsNoNExistTextBlock = new TextBlock
                {
                    Text = $"デフォルト作業項目はありません。",
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                };
                SpecialTasksListPanel.Children.Add(itemsNoNExistTextBlock);
            }
            else
            {
                SpecialTasksListPanel.Children.Add(grid);
            }

        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton)
            {
                return;
            }

            if (radioButton.Tag is not int specialTaskId)
            {
                return;
            }

            using var db = new AppDbContext();
            using var transaction = db.Database.BeginTransaction();
            try
            {
                var specialTasks = db.SpecialTasks.ToList();
                var now = DateTime.Now;

                foreach (var specialTask in specialTasks)
                {
                    bool newDefaultStartFlg =
                        specialTask.SpecialTaskId == specialTaskId;

                    // 値が実際に変わる場合だけ更新する
                    if (specialTask.DefaultStartFlg != newDefaultStartFlg)
                    {
                        specialTask.DefaultStartFlg = newDefaultStartFlg;
                        specialTask.UpdatedAt = now;
                    }
                }

                db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                ErrorLogger.Write(ex);

                transaction.Rollback();

                MessageBox.Show(
                    "ラジオボタン押下時のデータの保存に失敗しました。",
                    "エラー",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }
        }

        private void CheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is not CheckBox checkBox)
            {
                return;
            }

            if (checkBox.Tag is not int specialTaskId)
            {
                return;
            }

            bool isChecked = checkBox.IsChecked == true;

            using var db = new AppDbContext();

            var specialTask = db.SpecialTasks
                .FirstOrDefault(x => x.SpecialTaskId == specialTaskId);

            if (specialTask == null)
            {
                return;
            }

            specialTask.IsActive = isChecked;
            specialTask.UpdatedAt = DateTime.Now;

            db.SaveChanges();
        }

        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "本当に削除しますか？",
                "削除確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            if (sender is not Button button)
            {
                return;
            }

            if (button.Tag is not int specialTaskId)
            {
                return;
            }

            using var db = new AppDbContext();

            var specialTask = db.SpecialTasks
                .FirstOrDefault(x => x.SpecialTaskId == specialTaskId);

            if (specialTask == null)
            {
                return;
            }

            specialTask.IsDeleted = true;
            specialTask.UpdatedAt = DateTime.Now;

            db.SaveChanges();

            LoadSpecialTasks();
        }


        private void CloseSpecialTaskEntry_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
