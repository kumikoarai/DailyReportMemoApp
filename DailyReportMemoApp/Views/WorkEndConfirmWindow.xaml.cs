using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.ViewModels;
using System;
using System.Collections;
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
    /// WorkCompleteWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class WorkEndConfirmWindow : Window
    {
        private WorkingOn? _workingOnLogs = new();
        private WorkingOnRepository _workingOnRepository = new();
        private WorkLogsRepository _workLogsRepository = new();
        private ProjectTaskItemsRepository _projectTaskItemsRepository = new();
        private WorkTimeRangesRepository _workTimeRangesRepository = new();
        private Common _common = new();
        private TimeSpan _TodayEndWorkTime = new TimeSpan(23, 59, 59);
        private DateTime _now;

        public WorkEndConfirmWindow(WorkingOn workingOn)
        {
            InitializeComponent();

            _workingOnLogs = workingOn;

            TodayDate.Text = _workingOnLogs.WorkDate.ToString("yyyy年MM月dd日");

            _now = DateTime.Now;

            _TodayEndWorkTime = Common.workTimeSpan(_now, _workingOnLogs);

            // 作業中のタスクをロードする
            LoadTasks();
        }

        /// <summary>
        /// 作業タスクをロードするメソッド
        /// </summary>
        private void LoadTasks()
        {
            TasksListPanel.Children.Clear();

            if (_workingOnLogs == null)
            {
                return;
            }

            // 本日の作業ログを取得する
            var todayWorkLogs = _workLogsRepository.GetWorkLogs(_workingOnLogs.WorkingOnId);

            // 作業ログIDの配列を作成する
            int[] workLogIds = todayWorkLogs.Select(wl => wl.WorkLogId).ToArray();
            // 作業時間範囲を作業開始時間順に取得する
            var workTimeRangeOrderStartTimes = _workTimeRangesRepository.GetWorkTimeRangeOrderStartTime(workLogIds);
            // 作業時間範囲の最初と最後の作業開始時間を取得する
            var firstWorkTimeRangeOrderStartTime = workTimeRangeOrderStartTimes.FirstOrDefault();

            // 作業時間範囲の最初の作業開始時間が存在する場合、作業時間を計算して表示する
            if (firstWorkTimeRangeOrderStartTime?.StartTime != null)
            {
                TimeSpan duration = _common.RemoveSeconds(_TodayEndWorkTime) - _common.RemoveSeconds((TimeSpan)firstWorkTimeRangeOrderStartTime.StartTime);
                TodayWorkTimes.Text = $"本日の作業時間: {(int)duration.TotalHours}時間{duration.Minutes}分";
            }

            // RichTextBoxを作成する
            var richTextBox = new RichTextBox
            {
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };

            var document = new FlowDocument();

            var companyIds = -1;
            var projectIds = -1;
            TimeSpan worktimesCP = new TimeSpan(0, 0, 0);
            var title = new Paragraph{};
            var ttlNameForTime = "";

            foreach (var workLog in todayWorkLogs)
            {
                // 作業時間が存在するかどうかを確認する
                var timeExist = workLog.WorkTimeRanges.Any(w => w.StartTime != null);

                var ttlName = "";
                var taskName = "";
                var ttlOnly = false;

                // 作業時間が存在する場合のみ、会社名・プロジェクト名・タスク名を表示する
                if (timeExist) 
                {
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
                                worktimesCP = new TimeSpan(0, 0, 0);
                                ttlName = $"▼{companyName} / {projectName}";
                                taskName = $"・{taskItemName}";
                                ttlOnly = true;
                            }
                        }
                        else
                        {
                            worktimesCP = new TimeSpan(0, 0, 0);
                            ttlName = $"▼{companyName} / {projectName}";
                            taskName = $"・{taskItemName}";
                            ttlOnly = true;
                        }

                        companyIds = workLog.ProjectTaskItems?.CompanyProjects?.CompanyId ?? -1;
                        projectIds = workLog.ProjectTaskItems?.CompanyProjects?.ProjectId ?? -1;
                    }
                    else if (workLog.SpecialTaskId != null)
                    {
                        // 特別作業項目の場合
                        taskName = $"・{workLog.SpecialTasks?.SpecialTaskName ?? ""}";
                    }
                }

                // 会社名とプロジェクト名を表示する
                if (ttlName != "")
                {
                    ttlNameForTime = ttlName;   
                    title = new Paragraph
                    {
                        Margin = new Thickness(0, 20, 0, 5),
                        Padding = new Thickness(5),
                        Background = Brushes.AliceBlue
                    };
                    title.Inlines.Add(new Run(ttlName)
                    {
                        FontWeight = FontWeights.Bold,
                        FontSize = 16
                    });

                    document.Blocks.Add(title);
                }


                // タスク名を表示する
                var task = new Paragraph
                {
                    Margin = new Thickness(0, 5, 0, 5)
                };
                if (taskName != "") 
                {
                    task.Inlines.Add(new Run(taskName)
                    {
                        FontWeight = FontWeights.Bold,
                        FontSize = 14
                    });

                    document.Blocks.Add(task);
                }


                var time = new Paragraph
                {
                    Margin = new Thickness(0, 2, 0, 5)
                };
                var multipleExist = false;
                TimeSpan worktimes = new TimeSpan(0, 0, 0);
                foreach (var workTimeRange in workLog.WorkTimeRanges)
                {
                    // 作業時間が複数存在する場合は、スペースを追加する
                    if (multipleExist)
                    {
                        time.Inlines.Add(new Run("　"));
                    }

                    var endTime = "作業中";

                    if (workTimeRange.EndTime == null)
                    {
                        // 作業中の場合は、作業終了時間を本日の作業終了時間に設定する
                        int endHours = (int)_TodayEndWorkTime.TotalHours;
                        endTime = $"{endHours:00}:{_TodayEndWorkTime.Minutes:00}";
                    }
                    else
                    {
                        TimeSpan endTimeSpan = (TimeSpan)workTimeRange.EndTime;
                        int endHours = (int)endTimeSpan.TotalHours;
                        endTime = $"{endHours:00}:{endTimeSpan.Minutes:00}";
                    }

                    var startTime = "--:--";

                    if (workTimeRange.StartTime != null)
                    {
                        TimeSpan startTimeSpan = (TimeSpan)workTimeRange.StartTime;
                        int startHours = (int)startTimeSpan.TotalHours;
                        startTime = $"{startHours:00}:{startTimeSpan.Minutes:00}";

                        // 作業時間を計算する
                        if (workTimeRange.EndTime != null)
                        {
                            worktimes += _common.RemoveSeconds((TimeSpan)workTimeRange.EndTime) - _common.RemoveSeconds((TimeSpan)workTimeRange.StartTime);
                        }
                        else
                        {
                            worktimes += (_common.RemoveSeconds(_TodayEndWorkTime) - _common.RemoveSeconds((TimeSpan)workTimeRange.StartTime));
                        }
                    }

                    // 作業時間を表示する
                    time.Inlines.Add(new Run($"{startTime}～{endTime}")
                    {
                        Foreground = Brushes.Gray
                    });
                    document.Blocks.Add(time);

                    multipleExist = true;
                }

                // 作業合計時間を表示する
                time.Inlines.Add(new Run("　"));
                time.Inlines.Add(new Run($"({(int)worktimes.TotalHours}時間{worktimes.Minutes}分)")
                {
                    FontSize = 14
                });

                worktimesCP += worktimes;

                // 会社名・プロジェクト名の合計作業時間を表示する
                title.Inlines.Clear();

                title.Inlines.Add(new Run($"{ttlNameForTime}　({worktimesCP.Hours}時間{worktimesCP.Minutes}分)")
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16
                });
            }

            richTextBox.Document = document;

            //TasksListPanelに追加
            TasksListPanel.Children.Add(richTextBox);
        }


        private void WorkCompleted_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(
                "本日の作業を完了します。\nよろしいですか？",
                "作業完了の確認",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return;
            }

            if (_workingOnLogs == null)
            {
                return;
            }

            //案件作業項目の全ての本日対応フラグをfalseに更新
            _projectTaskItemsRepository.UpdateProjectTaskItemIsNotCurrent();

            var workLogsList = _workLogsRepository.GetSimpleWorkLogs(_workingOnLogs.WorkingOnId);


            foreach (var workLog in workLogsList)
            {
                foreach (var w_workTimeRange in workLog.WorkTimeRanges)
                {
                    // 他の作業が作業中の場合
                    if (w_workTimeRange.EndTime == null)
                    {
                        var updateEndTimeResult = _workTimeRangesRepository.UpdateWorkTimeRangeEndTime(w_workTimeRange, _now, _TodayEndWorkTime);
                        if (!updateEndTimeResult)
                        {
                            MessageBox.Show("タスク終了時間の更新に失敗しました。");
                            return;
                        }
                    }

                    //案件作業項目の対応中フラグを更新
                    if (w_workTimeRange.StartTime != null)
                    {
                        _projectTaskItemsRepository.UpdateProjectTaskItemIsCurrent(workLog.ProjectTaskItemId);
                    }
                }
            }

            var updateResult = _workingOnRepository.WorkingOnLogCompleted(_workingOnLogs);
            if (!updateResult)
            {
                MessageBox.Show("作業中のタスクの終了に失敗しました。");
                return;
            }
            //MessageBox.Show("作業中のタスクを終了しました。");


            DialogResult = true;
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
