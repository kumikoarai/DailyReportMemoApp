using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyReportMemoApp.Views
{
    /// <summary>
    /// PastWorkLogsPage.xaml の相互作用ロジック
    /// </summary>
    public partial class PastWorkLogsPage : Page
    {
        private WorkLogsRepository _workLogsRepository = new();
        private ViewModelsCommon _common = new();

        public PastWorkLogsPage(YearMonthItem? yearMonthItem, CompanyProjectItem? companyProjectItem)
        {
            InitializeComponent();

            if (yearMonthItem != null)
            {
                // 年月の作業ログを読み込む
                LoadYearMonthLogs(yearMonthItem);
            }
            if (companyProjectItem != null)
            {
                // 企業・案件の作業ログを読み込む
                LoadCompanyProjectLogs(companyProjectItem);
            }
        }


        /// <summary>
        /// 年月の作業ログを読み込むメソッド
        /// </summary>
        /// <param name="yearMonthItem"></param>
        private void LoadYearMonthLogs(YearMonthItem? yearMonthItem)
        {
            WorkLogsPanel.Children.Clear();

            if (yearMonthItem == null)
            {
                return;
            }

            // 年月の作業ログタイトルを表示する
            var TTLText = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 20),
                Text = $"{yearMonthItem.Year}年{yearMonthItem.Month}月の作業ログ",
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };

            // 年月の作業ログを取得する
            var groupedWorkLogsByYearMonth = _workLogsRepository.GetLogsByYearMonth(yearMonthItem.Year, yearMonthItem.Month);

            if (groupedWorkLogsByYearMonth == null)
            {
                return;
            }

            // RichTextBoxを作成する
            var richTextBox = new RichTextBox
            {
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };

            var document = new FlowDocument();
            TimeSpan worktimesCP = new TimeSpan(0, 0, 0);
            var datetitle = new Paragraph { };
            var ttlNameForTime = "";
            DateOnly logWorkDate = DateOnly.MinValue;

            foreach (var groupedWorkLog in groupedWorkLogsByYearMonth)
            {
                //日付けを表示する
                if (logWorkDate != groupedWorkLog.WorkDate)
                {
                    worktimesCP = new TimeSpan(0, 0, 0);
                    string date = groupedWorkLog.WorkDate.ToString(
                        "yyyy年M月d日（ddd）",
                        new CultureInfo("ja-JP")
                    );

                    ttlNameForTime = date;

                    datetitle = new Paragraph
                    {
                        Margin = new Thickness(0, 20, 0, 5),
                        Padding = new Thickness(5),
                        Background = Brushes.Silver
                    };
                    datetitle.Inlines.Add(new Run(date)
                    {
                        FontWeight = FontWeights.Bold,
                        FontSize = 16
                    });

                    document.Blocks.Add(datetitle);
                }

                logWorkDate = groupedWorkLog.WorkDate;

                // 作業ログの件数を取得する
                var count = groupedWorkLog.WorkLogListGroups
                                .Sum(x => x.WorkLogs
                                    .Sum(y => y.WorkTimeRanges.Count));

                // 作業ログが存在する場合は、会社名・プロジェクト名を表示する
                if (count > 0)
                {
                    var ttlName = "";
                    if (groupedWorkLog.CompanyProjectName != "" && groupedWorkLog.CompanyProjectName != null)
                    {
                        ttlName = $"◆{groupedWorkLog.CompanyProjectName}";
                    }
                    else
                    {
                        ttlName = $"◆{groupedWorkLog.SpecialTaskName}";
                    }

                    // 会社名・プロジェクト名を表示する
                    var workLogIdParagraph = new Paragraph
                    {
                        Margin = new Thickness(0, 0, 0, 0),
                        //Padding = new Thickness(5),
                        //Background = Brushes.AliceBlue
                    };
                    workLogIdParagraph.Inlines.Add(new Run(ttlName)
                    {
                        FontWeight = FontWeights.Bold,
                        FontSize = 16
                    });

                    document.Blocks.Add(workLogIdParagraph);
                }


                // 作業ログの作業時間を計算する
                foreach (var workLogListGroup in groupedWorkLog.WorkLogListGroups)
                {
                    TimeSpan worktimes = new TimeSpan(0, 0, 0);

                    var count2 = workLogListGroup.WorkLogs
                                        .Sum(y => y.WorkTimeRanges.Count);

                    if (count2 > 0)
                    {
                        if (workLogListGroup.ProjectTaskItemId != null)
                        {
                            // 作業内容を表示する
                            var workLogParagraph = new Paragraph
                            {
                                Margin = new Thickness(0, 0, 0, 0),
                                //Padding = new Thickness(5),
                                //Background = Brushes.LightYellow
                            };
                            workLogParagraph.Inlines.Add(new Run($"・{workLogListGroup.ProjectTaskItemName}")
                            {
                                FontWeight = FontWeights.Bold,
                                FontSize = 14
                            });
                            document.Blocks.Add(workLogParagraph);
                        }

                        var time = new Paragraph
                        {
                            Margin = new Thickness(20, 0, 0, 0)
                        };
                        var multipleExist = false;

                        foreach (var workLog in workLogListGroup.WorkLogs)
                        {

                            foreach (var workTimeRange in workLog.WorkTimeRanges)
                            {
                                // 作業時間が複数存在する場合は、スペースを追加する
                                if (multipleExist)
                                {
                                    time.Inlines.Add(new Run("　"));
                                }

                                var endTime = "作業中";

                                if (workTimeRange.EndTime != null)
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
                                }

                                // 作業時間を表示する
                                time.Inlines.Add(new Run($"{startTime}～{endTime}")
                                {
                                    Foreground = Brushes.Gray
                                });

                                // 作業時間が複数存在する場合は、スペースを追加する
                                multipleExist = true;
                            }

                        }
                        // 作業時間を表示する
                        document.Blocks.Add(time);

                        // 作業合計時間を表示する
                        time.Inlines.Add(new Run("　"));
                        time.Inlines.Add(new Run($"({(int)worktimes.TotalHours}時間{worktimes.Minutes}分)")
                        {
                            FontSize = 14
                        });

                        // 会社名・プロジェクト名の合計作業時間を計算する
                        worktimesCP += worktimes;
                    }
                }

                // 会社名・プロジェクト名の合計作業時間を表示する
                datetitle.Inlines.Clear();

                datetitle.Inlines.Add(new Run($"{ttlNameForTime}　({(int)worktimesCP.TotalHours}時間{worktimesCP.Minutes}分)")
                {
                    FontWeight = FontWeights.Bold,
                    FontSize = 16
                });
            }
            // RichTextBoxにFlowDocumentを設定する
            richTextBox.Document = document;

            // 年月の作業ログタイトルとRichTextBoxをWorkLogsPanelに追加する
            WorkLogsPanel.Children.Add(TTLText);
            WorkLogsPanel.Children.Add(richTextBox);
        }


        /// <summary>
        /// 企業・案件の作業ログを読み込むメソッド
        /// </summary>
        /// <param name="companyProjectItem"></param>
        private void LoadCompanyProjectLogs(CompanyProjectItem? companyProjectItem)
        {
            WorkLogsPanel.Children.Clear();

            if (companyProjectItem == null)
            {
                return;
            }

            var groupedWorkLogsByCompanyProject = _workLogsRepository.GetLogsByCompanyProject(companyProjectItem.CompanyId, companyProjectItem.ProjectId);

            if (groupedWorkLogsByCompanyProject == null)
            {
                return;
            }

            // RichTextBoxを作成する
            var richTextBox = new RichTextBox
            {
                IsReadOnly = true,
                BorderThickness = new Thickness(0),
                Background = Brushes.Transparent
            };

            var document = new FlowDocument();
            TimeSpan worktimesCP = new TimeSpan(0, 0, 0);
            var taskItemNameitle = new Paragraph { };
            var ttlNameForTime = "";
            var TTLProjectMemoText = new TextBox { };
            var projectMemo = "";
            TimeSpan workProjectTimes = new TimeSpan(0, 0, 0);

            foreach (var groupedWorkLog in groupedWorkLogsByCompanyProject)
            {
                foreach (var workLogListGroup2 in groupedWorkLog.WorkLogListGroups2)
                {
                    if (workLogListGroup2.ProjectTaskItemName != null && workLogListGroup2.ProjectTaskItemName != "")
                    {
                        worktimesCP = new TimeSpan(0, 0, 0);

                        string taskItemName = workLogListGroup2.ProjectTaskItemName;

                        ttlNameForTime = taskItemName;

                        // 作業内容を表示する
                        taskItemNameitle = new Paragraph
                        {
                            Margin = new Thickness(0, 0, 0, 0),
                            Padding = new Thickness(0),
                            Background = Brushes.Silver
                        };
                        taskItemNameitle.Inlines.Add(new Run(taskItemName)
                        {
                            FontWeight = FontWeights.Bold,
                            FontSize = 16
                        });

                        document.Blocks.Add(taskItemNameitle);


                        foreach (var workLogGroup3 in workLogListGroup2.WorkLogGroups3)
                        {

                            // 作業ログの件数を取得する
                            var count = workLogGroup3.WorkLogs
                                            .Sum(y => y.WorkTimeRanges.Count);

                            if (count > 0)
                            {
                                //日付けを表示する
                                var workLogPara = new Paragraph
                                {
                                    Margin = new Thickness(0, 0, 0, 0),
                                    Padding = new Thickness(0),
                                    //Background = Brushes.LightSteelBlue
                                };

                                string date = workLogGroup3.WorkDate.ToString(
                                                    "yyyy年M月d日（ddd）",
                                                    new CultureInfo("ja-JP")
                                                );

                                workLogPara.Inlines.Add(new Run(date)
                                {
                                    FontWeight = FontWeights.Bold,
                                    FontSize = 16
                                });
                                document.Blocks.Add(workLogPara);
                            }

                            // 作業内容を表示する
                            var time = new Paragraph
                            {
                                Margin = new Thickness(0, 0, 0, 0),
                                Padding = new Thickness(0),
                            };
                            var multipleExist = false;
                            TimeSpan worktimes = new TimeSpan(0, 0, 0);

                            foreach (var WorkLog in workLogGroup3.WorkLogs)
                            {
                                projectMemo = WorkLog.ProjectTaskItems!.CompanyProjects!.Memo;

                                foreach (var workTimeRange in WorkLog.WorkTimeRanges)
                                {
                                    // 作業時間が複数存在する場合は、スペースを追加する
                                    if (multipleExist)
                                    {
                                        time.Inlines.Add(new Run("　"));
                                    }

                                    var endTime = "作業中";

                                    if (workTimeRange.EndTime != null)
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
                                    }

                                    // 作業時間を表示する
                                    time.Inlines.Add(new Run($"{startTime}～{endTime}")
                                    {
                                        Foreground = Brushes.Gray
                                    });

                                    // 作業時間が複数存在する場合は、スペースを追加する
                                    multipleExist = true;

                                    // 作業時間を表示する
                                    document.Blocks.Add(time);

                                }

                            }
                            // 作業合計時間を表示する
                            time.Inlines.Add(new Run("　"));
                            time.Inlines.Add(new Run($"({(int)worktimes.TotalHours}時間{worktimes.Minutes}分)")
                            {
                                FontSize = 14
                            });

                            
                            worktimesCP += worktimes;

                        }

                        // 会社名・プロジェクト名の合計作業時間を表示する
                        taskItemNameitle.Inlines.Clear();

                        taskItemNameitle.Inlines.Add(new Run($"{ttlNameForTime}　({(int)worktimesCP.TotalHours}時間{worktimesCP.Minutes}分)")
                        {
                            FontWeight = FontWeights.Bold,
                            FontSize = 16
                        });

                        workProjectTimes += worktimesCP;

                    }
                }

            }

            // RichTextBoxにFlowDocumentを設定する
            richTextBox.Document = document;

            // 企業の作業ログタイトルとRichTextBoxをWorkLogsPanelに追加する
            var TTLCompanyText = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 0),
                Text = $"【{companyProjectItem.CompanyName}】",
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                FontWeight = FontWeights.Bold
            };
            WorkLogsPanel.Children.Add(TTLCompanyText);

            // 案件の作業ログタイトルとRichTextBoxをWorkLogsPanelに追加する
            var TTLProjectText = new TextBlock
            {
                Margin = new Thickness(0, 0, 0, 10),
                Padding = new Thickness(5),
                Text = $"{companyProjectItem.ProjectName}　({(int)workProjectTimes.TotalHours}時間{workProjectTimes.Minutes}分)",
                VerticalAlignment = VerticalAlignment.Center,
                TextWrapping = TextWrapping.Wrap,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Background = Brushes.LightBlue,
            };
            WorkLogsPanel.Children.Add(TTLProjectText);

            if (projectMemo != "" && projectMemo != null)
            {
                //案件のメモを表示する
                TTLProjectMemoText = new TextBox
                {
                    Text = $"{projectMemo}",
                    Margin = new Thickness(0, 0, 0, 20),
                    MinHeight = 20,
                    AcceptsReturn = true,          // Enterで改行
                    TextWrapping = TextWrapping.Wrap, // 長い文章は折り返す
                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled,
                    IsReadOnly = true
                };
                WorkLogsPanel.Children.Add(TTLProjectMemoText);
            }

            // RichTextBoxをWorkLogsPanelに追加する
            WorkLogsPanel.Children.Add(richTextBox);
        }

    }
}
