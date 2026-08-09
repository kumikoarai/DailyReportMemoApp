using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.ViewModels;
using DailyReportMemoApp.Views;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyReportMemoApp
{
    /// <summary>
    /// WorkEntryWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkEntryWindow : Window
    {
        public WorkingOn? _wOLog { get; private set; }
        private WorkingOn? _workingOnLogs = new ();
        private WorkingOnRepository _workingOnRepository = new();
        private WorkLogsRepository _workLogsRepository = new();
        private SpecialTasksRepository _specialTasksRepository = new();
        private ProjectTaskItemsRepository _projectTaskItemsRepository = new();
        private WorkTimeRangesRepository _workTimeRangesRepository = new();
        private CompanyProjectsRepository _companyProjectsRepository = new();
        private readonly List<RadioButton> _radioButtons = new();
        private bool _skipLostFocus;
        private ChangeWorkTimeRange _changeWorkTimeRange = new ChangeWorkTimeRange();
        private Common _common = new();


        public WorkEntryWindow()
        {
            InitializeComponent();

            _workingOnLogs = _workingOnRepository.GetWorkingOnLogs();

            var nonToday = false;
            // 作業中のタスクが存在しない場合、本日の作業中の記録を開始する
            if (_workingOnLogs == null)
            {
                WorkingOn newWorkingOn = WorkingOnRecordingStarted();
                _workingOnLogs = newWorkingOn;

                nonToday = true;
            }

            TodayDate.Text = _workingOnLogs.WorkDate.ToString("yyyy年MM月dd日");

            // 今日の作業ログを取得する
            var todayWorkLogs = _workLogsRepository.GetWorkLogs(_workingOnLogs.WorkingOnId);

            // 作業中のタスクが存在しない場合、特別作業項目の作業ログを追加する
            AddWorkLogsForSpecialTasks(todayWorkLogs);

            // 作業中のタスクが存在しない場合、案件作業項目の作業ログを追加する
            AddWorkLogsForProjectTasks(todayWorkLogs);

            // 作業中のタスクをロードする
            LoadTasks();

            //作業中のタスクが存在しない場合
            if (nonToday)
            {
                //デフォルトで開始する特別作業のラジオボタンにチェックを入れる。
                LoadDefaultStart();
            }
        }

        /// <summary>
        /// 作業中のタスクが存在しない場合、特別作業項目の作業ログを追加するメソッド
        /// </summary>
        private void AddWorkLogsForSpecialTasks(List<WorkLog> todayWorkLogs)
        {
            if (_workingOnLogs == null)
            {
                return;
            }

            var specialTasks = _specialTasksRepository.GetSpecialTasks();

            if (todayWorkLogs.Count <= 0)
            {
                // 今日の作業ログが存在しない場合、特別作業項目の作業ログを新規作成する
                for (var st = 0; st < specialTasks.Count; st++)
                {
                    var specialTask = specialTasks[st];

                    var workLog = new WorkLog
                    {
                        SpecialTaskId = specialTask.SpecialTaskId,
                        WorkingOnId = _workingOnLogs.WorkingOnId,
                        CreatedAt = DateTime.Now
                    };
                    _workLogsRepository.AddWorkLog(workLog);
                }
            }
            else
            {
                // 今日の作業ログが存在する場合、特別作業項目の作業ログが存在しない場合は新しい作業ログを作成する
                foreach (var specialTask in specialTasks)
                {
                    var existingWorkLog = todayWorkLogs.FirstOrDefault(wl => wl.SpecialTaskId == specialTask.SpecialTaskId);
                    if (existingWorkLog == null)
                    {
                        var newWorkLog = new WorkLog
                        {
                            SpecialTaskId = specialTask.SpecialTaskId,
                            WorkingOnId = _workingOnLogs.WorkingOnId,
                            CreatedAt = DateTime.Now
                        };
                        _workLogsRepository.AddWorkLog(newWorkLog);
                    }
                }
            }
        }

        /// <summary>
        /// 作業中のタスクが存在しない場合、案件作業項目の作業ログを追加するメソッド
        /// </summary>
        /// <param name="todayWorkLogs"></param>
        private void AddWorkLogsForProjectTasks(List<WorkLog> todayWorkLogs)
        {
            if (_workingOnLogs == null)
            {
                return;
            }

            var currentProjectTasks = _projectTaskItemsRepository.GetCurrentProjectTaskItems();

            if (todayWorkLogs.Count <= 0)
            {
                // 今日の作業ログが存在しない場合、案件作業項目の作業ログを新規作成する
                for (var pt = 0; pt < currentProjectTasks.Count; pt++)
                {
                    var currentProjectTask = currentProjectTasks[pt];

                    var workLog = new WorkLog
                    {
                        ProjectTaskItemId = currentProjectTask.ProjectTaskItemId,
                        WorkingOnId = _workingOnLogs.WorkingOnId,
                        CreatedAt = DateTime.Now
                    };
                    _workLogsRepository.AddWorkLog(workLog);
                }
            }
            else
            {
                // 今日の作業ログが存在する場合、案件作業項目の作業ログが存在しない場合は新しい作業ログを作成する
                foreach (var currentProjectTask in currentProjectTasks)
                {
                    var existingWorkLog = todayWorkLogs.FirstOrDefault(wl => wl.ProjectTaskItemId == currentProjectTask.ProjectTaskItemId);
                    if (existingWorkLog == null)
                    {
                        var newWorkLog = new WorkLog
                        {
                            ProjectTaskItemId = currentProjectTask.ProjectTaskItemId,
                            WorkingOnId = _workingOnLogs.WorkingOnId,
                            CreatedAt = DateTime.Now
                        };
                        _workLogsRepository.AddWorkLog(newWorkLog);
                    }
                }
            }
        }


        /// <summary>
        /// 本日の作業の記録を開始するメソッド
        /// </summary>
        private WorkingOn WorkingOnRecordingStarted()
        {
            var now = DateTime.Now;

            var workingOn = new WorkingOn
            {
                WorkingOnFlg = true,
                WorkDate = DateOnly.FromDateTime(now),
                WorkingOnStart = now,
                CreatedAt = now
            };

            return _workingOnRepository.AddWorkingOn(workingOn);
        }


        /// <summary>
        /// 作業タスクをロードするメソッド
        /// </summary>
        private void LoadTasks() 
        {
            TasksListPanel.Children.Clear();

            var grid = new Grid
            {
                Margin = new Thickness(5)
            };

            // 1列目：使用する項目のチェックボックス用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });

            // 1列目：作業項目用
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });

            // 0行目をタイトル行として追加
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = GridLength.Auto
            });

            // タイトルを作成
            var activeTitle = new TextBlock
            {
                Text = "作業中",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            var tasksTitle = new TextBlock
            {
                Text = "",
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5)
            };

            // タイトルの配置
            Grid.SetRow(activeTitle, 0);
            Grid.SetColumn(activeTitle, 0);

            Grid.SetRow(tasksTitle, 0);
            Grid.SetColumn(tasksTitle, 1);

            // タイトルをGridに追加する
            grid.Children.Add(activeTitle);
            grid.Children.Add(tasksTitle);

            if (_workingOnLogs == null)
            {
                return;
            }

            // 今日の作業ログを取得する
            var todayWorkLogs = _workLogsRepository.GetWorkLogs(_workingOnLogs.WorkingOnId);


            var companyIds = -1;
            var projectIds = -1;

            int rowIndex = 0;

            foreach (var workLog in todayWorkLogs)
            {
                var currentTask = false;

                // 0行目はタイトルなので、データ行は1行目から
                rowIndex++;

                var timeRowPanelWrap = new WrapPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(20, 2, 0, 2)
                };

                foreach (var workTimeRange in workLog.WorkTimeRanges)
                {
                    var timeRowPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 2, 0, 2)
                    };

                    var endTime = "作業中";

                    if (workTimeRange.EndTime == null)
                    {
                        currentTask = true;
                    }
                    else
                    {
                        TimeSpan endTimeSpan = (TimeSpan)workTimeRange.EndTime;
                        int endHours = (int)endTimeSpan.TotalHours;
                        endTime = $"{endHours:00}:{endTimeSpan.Minutes:00}";
                    }

                    var startTime = "--:--";

                    if (workTimeRange.StartTime != null) {
                        TimeSpan startTimeSpan = (TimeSpan)workTimeRange.StartTime;
                        int startHours = (int)startTimeSpan.TotalHours;
                        startTime = $"{startHours:00}:{startTimeSpan.Minutes:00}";
                    }

                    var sagyochu = false;
                    if (endTime == "作業中")
                    {
                        sagyochu = true;
                    }

                    var startTimeText = new TextBox
                    {
                        Text = $"{startTime}",
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        Background = Brushes.White
                    };
                    startTimeText.Tag = new WorkTime
                    {
                        StartOrEnd = "start",
                        WorkTimeRange = workTimeRange,
                        Sagyochu = sagyochu
                    };
                    startTimeText.LostFocus += TimeTextBox_LostFocus;
                    startTimeText.KeyDown += TimeTextBox_KeyDown;
                    timeRowPanel.Children.Add(startTimeText);

                    if (_changeWorkTimeRange.WorkTimeRangeId == workTimeRange.WorkTimeRangeId && _changeWorkTimeRange.SubTargetStartOrEnd == "start") {
                        _changeWorkTimeRange.TextBox = startTimeText;
                    }

                    var enDashText = new TextBlock
                    {
                        Text = $" ～ ",
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap
                    };
                    timeRowPanel.Children.Add(enDashText);

                    if (sagyochu)
                    {
                        var endTimeText = new TextBlock
                        {
                            Text = $"{endTime}",
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap
                        };
                        timeRowPanel.Children.Add(endTimeText);

                        var delButton = new Button
                        {
                            Content = "×",
                            Height = 20,
                            VerticalAlignment = VerticalAlignment.Center
                        };

                        delButton.Tag = new WorkTime
                        {
                            StartOrEnd = "start",
                            WorkTimeRange = workTimeRange,
                            Sagyochu = sagyochu
                        };

                        delButton.Click += TimeDelButton_click;

                        timeRowPanel.Children.Add(delButton);

                    }
                    else
                    {
                        var endTimeText = new TextBox
                        {
                            Text = $"{endTime}",
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            Background = Brushes.White
                        };

                        endTimeText.Tag = new WorkTime
                        {
                            StartOrEnd = "end",
                            WorkTimeRange = workTimeRange,
                            Sagyochu = sagyochu
                        };

                        endTimeText.LostFocus += TimeTextBox_LostFocus;

                        endTimeText.KeyDown += TimeTextBox_KeyDown;

                        timeRowPanel.Children.Add(endTimeText);

                        if (_changeWorkTimeRange.WorkTimeRangeId == workTimeRange.WorkTimeRangeId && _changeWorkTimeRange.SubTargetStartOrEnd == "end")
                        {
                            _changeWorkTimeRange.TextBox = endTimeText;
                        }

                        var slashText = new TextBlock
                        {
                            Text = $"/",
                            VerticalAlignment = VerticalAlignment.Center,
                            TextWrapping = TextWrapping.Wrap,
                            Margin = new Thickness(20, 0, 20, 0)
                        };

                        timeRowPanel.Children.Add(slashText);

                    }

                    //var editButton = new Button
                    //{
                    //    Content = "修正",
                    //    Margin = new Thickness(10, 0, 0, 0),
                    //    Padding = new Thickness(8, 2, 8, 2),

                    //    // 押された時間帯を特定するため、IDを保持
                    //    Tag = workTimeRange.WorkTimeRangeId
                    //};
                    //editButton.Click += WorkTimeRangeEdit_Click;

                    // 作業時間帯の表示
                    //timeRowPanel.Children.Add(editButton);

                    // 作業時間帯のWrapPanelに追加
                    timeRowPanelWrap.Children.Add(timeRowPanel);
                }


                var ttlName = "";
                var taskName = "";
                var ttlOnly = false;

                // 作業項目が案件作業項目の場合
                if (workLog.ProjectTaskItemId != null)
                {
                    var companyName = workLog.ProjectTaskItems?.CompanyProjects?.Companies?.CompanyName ?? "";
                    var projectName = workLog.ProjectTaskItems?.CompanyProjects?.Projects?.ProjectName ?? "";
                    var taskItemName = workLog.ProjectTaskItems?.TaskItems?.TaskItemName ?? "";

                    if (companyIds == workLog.ProjectTaskItems?.CompanyProjects?.CompanyId)
                    {
                        if (projectIds == workLog.ProjectTaskItems?.CompanyProjects?.ProjectId)
                        {
                            taskName = $"・{taskItemName}";
                        }
                        else
                        {
                            ttlName = $"{companyName} / {projectName}";
                            taskName = $"・{taskItemName}";
                            ttlOnly = true;
                        }
                    }
                    else
                    {
                        ttlName = $"{companyName} / {projectName}";
                        taskName = $"・{taskItemName}";
                        ttlOnly = true;
                    }

                    companyIds = workLog.ProjectTaskItems?.CompanyProjects?.CompanyId ?? -1;
                    projectIds = workLog.ProjectTaskItems?.CompanyProjects?.ProjectId ?? -1;
                }
                else if (workLog.SpecialTaskId != null)
                {
                    // 特別作業項目の場合

                    taskName = workLog.SpecialTasks?.SpecialTaskName ?? "";
                }


                var radioButton = new RadioButton
                {
                    GroupName = "TaskGroup",
                    Margin = new Thickness(5),
                    Tag = workLog,
                    IsChecked = currentTask,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                radioButton.Checked += RadioButton_Checked;


                // タイトル行を追加する場合
                if (ttlOnly)
                {
                    // 1件につき1行追加
                    grid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });

                    var border = new Border
                    {
                        Background = Brushes.AliceBlue,
                        Padding = new Thickness(5),
                        CornerRadius = new CornerRadius(4)
                    };

                    var textPanelTtl = new StackPanel
                    {
                        //Orientation = Orientation.Horizontal,
                        Margin = new Thickness(0, 2, 0, 2)
                    };

                    textPanelTtl.Children.Add(new TextBox
                    {
                        Text = $"{ttlName}",
                        IsReadOnly = true,
                        Margin = new Thickness(5),
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.Wrap,
                        BorderThickness = new Thickness(0),
                        Background = Brushes.Transparent,
                        IsReadOnlyCaretVisible = false
                    });

                    border.Child = textPanelTtl;

                    // タイトル行の配置
                    Grid.SetRow(border, rowIndex);
                    Grid.SetColumn(border, 0);
                    Grid.SetColumnSpan(border, 2);

                    grid.Children.Add(border);

                    rowIndex++;

                    // 1件につき1行追加
                    grid.RowDefinitions.Add(new RowDefinition
                    {
                        Height = GridLength.Auto
                    });


                    var inputGrid = new Grid
                    {
                        Margin = new Thickness(5)
                    };

                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    });

                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = new GridLength(1, GridUnitType.Star)
                    });

                    inputGrid.ColumnDefinitions.Add(new ColumnDefinition
                    {
                        Width = GridLength.Auto
                    });

                    var memoTtl = new TextBlock
                    {
                        Text = "メモ",
                        //VerticalAlignment = VerticalAlignment.Center,
                        Height = 20,
                    };

                    var memoTextBox = new TextBox
                    {
                        Text = workLog.ProjectTaskItems?.CompanyProjects?.Memo ?? "",
                        Margin = new Thickness(5),
                        MinHeight = 20,
                        AcceptsReturn = true,          // Enterで改行
                        TextWrapping = TextWrapping.Wrap, // 長い文章は折り返す
                        VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                        HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
                    };

                    var saveButton = new Button
                    {
                        Content = "保存",
                        Height = 20,
                        VerticalAlignment = VerticalAlignment.Center
                    };

                    saveButton.Tag = new MemoSaveInfo
                    {
                        WorkLog = workLog,
                        TextBox = memoTextBox
                    };

                    saveButton.Click += MemoSaveButton_Click;

                    Grid.SetColumn(memoTtl, 0);
                    Grid.SetColumn(memoTextBox, 1);
                    Grid.SetColumn(saveButton, 2);

                    inputGrid.Children.Add(memoTtl);
                    inputGrid.Children.Add(memoTextBox);
                    inputGrid.Children.Add(saveButton);

                    Grid.SetRow(inputGrid, rowIndex);
                    Grid.SetColumn(inputGrid, 0);
                    Grid.SetColumnSpan(inputGrid, 3);

                    grid.Children.Add(inputGrid);

                    rowIndex++;
                }

                // 1件につき1行追加
                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                var textPanel = new StackPanel
                {
                    //Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 2, 0, 2)
                };

                textPanel.Children.Add(new TextBox
                {
                    Text = $"{taskName}",
                    Margin = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    IsReadOnly = true,
                    BorderThickness = new Thickness(0),
                    Background = Brushes.Transparent,
                    IsReadOnlyCaretVisible = false
                });


                // ラジオボタンと作業項目の配置
                Grid.SetRow(radioButton, rowIndex);
                Grid.SetColumn(radioButton, 0);

                Grid.SetRow(textPanel, rowIndex);
                Grid.SetColumn(textPanel, 1);

                grid.Children.Add(radioButton);
                grid.Children.Add(textPanel);


                rowIndex++;

                grid.RowDefinitions.Add(new RowDefinition
                {
                    Height = GridLength.Auto
                });

                // 作業時間帯の配置
                Grid.SetRow(timeRowPanelWrap, rowIndex);
                Grid.SetColumn(timeRowPanelWrap, 1);

                grid.Children.Add(timeRowPanelWrap);

                _radioButtons.Add(radioButton);

            }

            //グリッドをTasksListPanelに追加
            TasksListPanel.Children.Add(grid);

            FlashTextBox(_changeWorkTimeRange);

            _changeWorkTimeRange.SuccessOrFailure = "";
            _changeWorkTimeRange.SubTargetStartOrEnd = "";
            _changeWorkTimeRange.WorkTimeRangeId = -1;
            _changeWorkTimeRange.TextBox = null;

        }

        /// <summary>
        /// ラジオボタンがチェックされたときのイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is not RadioButton radioButton)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (radioButton.Tag is not WorkLog tagWorkLog)
            {
                return;
            }

            if (_workingOnLogs == null)
            {
                return;
            }

            var workLogsList = _workLogsRepository.GetSimpleWorkLogs(_workingOnLogs.WorkingOnId);

            var now = DateTime.Now;

            TimeSpan nowTime = Common.workTimeSpan(now, _workingOnLogs);

            foreach (var workLog in workLogsList)
            {
                foreach (var w_workTimeRange in workLog.WorkTimeRanges)
                {
                    // 他の作業が作業中の場合
                    if (w_workTimeRange.EndTime == null)
                    {
                        var updateResult = _workTimeRangesRepository.UpdateWorkTimeRangeEndTime(w_workTimeRange, now, nowTime);
                        if(!updateResult)
                        {
                            MessageBox.Show("タスク終了時間の更新に失敗しました。");
                            return;
                        }
                    }
                }
            }

            var newWorkTimeRange = new WorkTimeRange
            {
                WorkLogId = tagWorkLog.WorkLogId,
                StartTime = nowTime,
                CreatedAt = now
            };
            _workTimeRangesRepository.AddWorkTimeRange(newWorkTimeRange);

            LoadTasks();
        }



        /// <summary>
        /// メモを保存する処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MemoSaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button ||
                button.Tag is not MemoSaveInfo saveInfo)
            {
                return;
            }

            //会社案件のメモを更新
            var rsltSaveInfo = _companyProjectsRepository.UpdateCompanyProjectMemo(saveInfo);

            if (rsltSaveInfo == null)
            {
                return;
            }
            saveInfo = rsltSaveInfo;

            MessageBox.Show("メモを保存しました。");
        }


        /// <summary>
        /// 「案件追加」ボタンがクリックされたときのイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenProjectEntryWindow_Click(object sender, RoutedEventArgs e)
        {

            ProjectEntryWindow projectEntryWindow = new()
            {
                Owner = this
            };

            if(projectEntryWindow.ShowDialog() == true)
            {
                LoadTasks();
            }

        }

        /// <summary>
        /// 作業時間範囲の入力欄に入力後フォーカスアウトしたら保存する
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_skipLostFocus)
            {
                _skipLostFocus = false;
                return;
            }

            if (sender is not TextBox timeTextBox)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (timeTextBox.Tag is not WorkTime workTime)
            {
                return;
            }

            ChangeTimeRange(timeTextBox, workTime);
            LoadTasks();

        }

        /// <summary>
        /// 作業時間範囲の入力欄に入力後enterキーを押下したときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            if (sender is not TextBox timeTextBox ||
                timeTextBox.Tag is not WorkTime workTime)
            {
                return;
            }

            // Enterキーの標準動作を止める
            e.Handled = true;
            _skipLostFocus = true;

            ChangeTimeRange(timeTextBox, workTime);
            LoadTasks();
        }

        /// <summary>
        /// 作業範囲の削除ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeDelButton_click(object sender, RoutedEventArgs e) 
        {
            if (sender is not Button timeDelButton)
            {
                return;
            }

            // 選択されたラジオボタンの作業ログを取得
            if (timeDelButton.Tag is not WorkTime workTime)
            {
                return;
            }

            ChangeTimeRange(null, workTime);
            LoadTasks();
        }

        /// <summary>
        /// 作業時間範囲の変更処理
        /// </summary>
        /// <param name="timeTextBox"></param>
        /// <param name="workTime"></param>
        private void ChangeTimeRange(TextBox? timeTextBox, WorkTime workTime) 
        {
            if (_workingOnLogs == null)
            {
                return;
            }

            var newTime = "";
            if (timeTextBox != null)
            {
                newTime = timeTextBox.Text.Trim();
            }


            if (string.IsNullOrWhiteSpace(newTime) || timeTextBox == null)
            {
                if (workTime.Sagyochu)
                {
                    MessageBoxResult result = MessageBox.Show(
                        "作業時間を削除します。\nよろしいですか？",
                        "作業時間削除の確認",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.No)
                    {
                        return;
                    }
                    if (!_workTimeRangesRepository.DeleteWorkTimeRange(workTime.WorkTimeRange.WorkTimeRangeId))
                    {
                        MessageBox.Show("作業範囲の削除に失敗しました。");
                        return;
                    }

                    if (!_workTimeRangesRepository.UpdateWorkTimeRangeTimeEndTimeDel(workTime.WorkTimeRange, _workingOnLogs.WorkingOnId))
                    {
                        MessageBox.Show("作業範囲の削除に失敗しました。");
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("作業時間が入力されていません。");

                }
            }
            else
            {

                // newTimeが全角文字や記号や数字を含む場合、半角に変換する
                newTime = newTime.Replace("：", ":").Replace("０", "0").Replace("１", "1").Replace("２", "2")
                                 .Replace("３", "3").Replace("４", "4").Replace("５", "5").Replace("６", "6")
                                 .Replace("７", "7").Replace("８", "8").Replace("９", "9");


                // newTimeが数字以外の文字を含む場合、エラーメッセージを表示して処理を終了する

                if (!System.Text.RegularExpressions.Regex.IsMatch(newTime, @"^\d{1,2}:\d{2}$"))
                {
                    MessageBox.Show("作業時間の形式が正しくありません。HH:mm形式で入力してください。");
                    return;
                }

                //// newTimeが24時間表記の範囲外の場合、エラーメッセージを表示して処理を終了する
                //if (timeTextBox != null)
                //{
                //    var timeParts = newTime.Split(':');
                //    if (timeParts.Length != 2 ||
                //        !int.TryParse(timeParts[0], out int hours) ||
                //        !int.TryParse(timeParts[1], out int minutes) ||
                //        hours < 0 || hours > 23 ||
                //        minutes < 0 || minutes > 59)
                //    {
                //        MessageBox.Show("作業時間の形式が正しくありません。HH:mm形式で入力してください。");
                //        return;
                //    }
                //}

                // newTimeが開始時間よりも終了時間が早い場合、エラーメッセージを表示して処理を終了する
                if (workTime.StartOrEnd == "end" && workTime.WorkTimeRange.StartTime != null)
                {
                    bool rslt = _common.TryParseTime(newTime, out TimeSpan parsedNewTime1);
                    var startTime = workTime.WorkTimeRange.StartTime.Value;
                    var endTime = parsedNewTime1;
                    if (endTime < startTime)
                    {
                        MessageBox.Show("終了時間は開始時間よりも後に設定してください。");
                        return;
                    }
                }
                else if (workTime.StartOrEnd == "start" && workTime.WorkTimeRange.EndTime != null)
                {
                    var startTime = TimeSpan.Parse(newTime);
                    var endTime = workTime.WorkTimeRange.EndTime.Value;
                    if (startTime > endTime)
                    {
                        MessageBox.Show("開始時間は終了時間よりも前に設定してください。");
                        return;
                    }
                }

                // バリデーションチェック
                if (!_common.TryParseTime(newTime, out TimeSpan parsedNewTime2))
                {
                    MessageBox.Show("作業時間の形式が正しくありません。HH:mm形式で入力してください。");
                    return;
                }


                _changeWorkTimeRange = _workTimeRangesRepository.UpdateWorkTimeRangeTime(workTime, newTime, _workingOnLogs.WorkingOnId);

                //作業時間の更新に失敗した場合、エラーメッセージを表示する
                if (_changeWorkTimeRange.SuccessOrFailure == "f")
                {
                    MessageBox.Show("作業時間の更新に失敗しました。");

                    return;
                }
            }


        }


        /// <summary>
        /// 作業完了ボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WorkCompleted_Click(object sender, RoutedEventArgs e)
        {

            _wOLog = _workingOnLogs;

            var confirmWindow = new WorkEndConfirmWindow(_wOLog)
            {
                Owner = this
            };

            bool? conRslt = confirmWindow.ShowDialog();

            if (conRslt == true)
            {
                Close();
            }
        }


        /// <summary>
        /// デフォルトで開始する特別作業のラジオボタンにチェックを入れる。
        /// </summary>
        private void LoadDefaultStart()
        {
            var specialTask = _specialTasksRepository.GetDefaulSpecialTasks();

            var radioButton = _radioButtons.FirstOrDefault(rb =>
                                rb.Tag is WorkLog workLog &&
                                workLog.SpecialTaskId == specialTask?.SpecialTaskId);

            if (radioButton != null)
            {
                //radioButton.IsChecked = true;
                RadioButton_Checked(radioButton, new RoutedEventArgs());
            }
        }

        /// <summary>
        /// テキストボックスを点滅させるアニメーションを実行するメソッド
        /// </summary>
        /// <param name="textBox"></param>
        private void FlashTextBox(ChangeWorkTimeRange? changeWorkTimeRange)
        {
            if (changeWorkTimeRange == null || changeWorkTimeRange.TextBox == null || changeWorkTimeRange.SuccessOrFailure == "" || changeWorkTimeRange.SuccessOrFailure == null || changeWorkTimeRange.WorkTimeRangeId == -1)
            {
                return;
            }

            var color = changeWorkTimeRange.SuccessOrFailure == "s" ? Colors.LightGreen : Colors.IndianRed;
            TextBox targetTexbox = changeWorkTimeRange.TextBox;

            Dispatcher.BeginInvoke(() =>
            {
                var brush = new SolidColorBrush(Colors.White);
                targetTexbox.Background = brush;

                var animation = new ColorAnimation
                {
                    From = Colors.White,
                    To = color,
                    Duration = TimeSpan.FromSeconds(1),
                    AutoReverse = true
                };

                brush.BeginAnimation(
                    SolidColorBrush.ColorProperty,
                    animation
                );
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        /// <summary>
        /// 画面を閉じるボタン押下時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }

}
