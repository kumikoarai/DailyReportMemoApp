using DailyReportMemoApp.Models;
using DailyReportMemoApp.Repositories;
using DailyReportMemoApp.Services;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DailyReportMemoApp.Views
{
    /// <summary>
    /// ProjectManagementPage.xaml の相互作用ロジック
    /// </summary>
    public partial class ProjectManagementPage : Page
    {
        private CompanyProjectsRepository _companyProjectsRepository = new();
        private ProjectTaskItemsRepository _projectTaskItemsRepository = new();
        private ProjectsRepository _projectsRepository = new();
        private TaskItemsRepository _taskItemsRepository = new();
        private Company _company = new();
        private ProjectManagement _projectManagement = new();

        public ProjectManagementPage(Company company)
        {
            InitializeComponent();

            _company = company;
            LoadProjects();
        }

        /// <summary>
        /// 会社に関連する案件のリストを取得してDataGridにバインドするメソッド
        /// </summary>
        private void LoadProjects()
        {
            // 選択された会社に関連する案件のリストを取得してDataGridにバインドする
            ProjectsListBox.ItemsSource = _companyProjectsRepository.GetCompanyProjectsFromManagement(_company.CompanyId);
        }

        /// <summary>
        /// 案件に関連する作業のリストを取得してDataGridにバインドするメソッド
        /// </summary>
        /// <param name="companyProjectId"></param>
        private void LoadProjectTaskItems(int companyProjectId)
        {
            // 選択された会社に関連する案件のリストを取得してDataGridにバインドする
            TaskItemsListBox.ItemsSource = _projectTaskItemsRepository.GetProjectTaskItems(companyProjectId);
        }


        /// <summary>
        /// 案件リストの選択が変更されたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 選択された案件を取得する
            var selectedProject = ProjectsListBox.SelectedItem as CompanyProject;

            // 選択された案件がnullの場合、作業リストをクリアして無効化する
            if (selectedProject == null)
            {
                TaskItemsListBox.ItemsSource = null;
                TaskItemsListBox.IsEnabled = false;
                ProjectReNameBorder.Visibility = Visibility.Collapsed;
                TaskItemsListBorder.Visibility = Visibility.Collapsed;
                ProjectSaveBtn.Visibility = Visibility.Collapsed;

                return;
            }

            // 選択された案件に関連する作業のリストを取得してDataGridにバインドする
            LoadProjectTaskItems(selectedProject.CompanyProjectId);
            ProjectReNameBox.Text = selectedProject.Projects?.ProjectName ?? "";
            ProjectsMemoPanel.Text = selectedProject.Memo ?? "";
            ProjectReNameBox.Tag = selectedProject.Projects?.ProjectId;
            ProjectsMemoPanel.Tag = selectedProject.CompanyProjectId;
            CompletedBox.IsChecked = selectedProject.Projects?.Completed;

            // 作業のリストを有効化する
            TaskItemsListBox.IsEnabled = true;
            TaskItemName.IsEnabled = true;
            TaskItemNameBtn.IsEnabled = true;
            //アニメーションで表示する
            ShowBorders(
                ProjectReNameBorder,
                TaskItemsListBorder
                );
            ProjectSaveBtn.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 案件名を入力して追加するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectEntry_Click(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectName.Text.Trim();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show("案件名を入力してください。");
                return;
            }


            // 選択された会社を取得する
            var selectedCompany = _company;
            if (selectedCompany == null)
            {
                MessageBox.Show("会社を選択してください。");
                return;
            }
            else
            {

                // 会社に関連する案件を追加する
                var project = new Project
                {
                    ProjectName = projectName,
                    Completed = false,
                    CreatedAt = DateTime.Now
                };
                var newProject = _projectsRepository.AddProject(project);

                // 会社と案件の関連を追加する
                var companyProject = new CompanyProject
                {
                    CompanyId = selectedCompany.CompanyId,
                    ProjectId = newProject.ProjectId,
                    CreatedAt = DateTime.Now
                };
                _companyProjectsRepository.AddCompanyProject(companyProject);
            }

            LoadProjects();
            ProjectName.Text = "";
        }


        /// <summary>
        /// 作業名を入力して追加するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskItemEntry_Click(object sender, RoutedEventArgs e)
        {
            var taskItemName = TaskItemName.Text.Trim();

            if (string.IsNullOrWhiteSpace(taskItemName))
            {
                MessageBox.Show("作業名を入力してください。");
                return;
            }

            var selectedCompany = _company;
            var selectedProject = ProjectsListBox.SelectedItem as CompanyProject;

            if (selectedCompany == null)
            {
                MessageBox.Show("会社を選択してください。");
                return;
            }

            if (selectedProject == null)
            {
                MessageBox.Show("案件を選択してください。");
                return;
            }

            // 選択された案件に関連する作業を追加する
            var taskItem = new TaskItem
            {
                TaskItemName = taskItemName,
                CreatedAt = DateTime.Now
            };
            var newTaskItem = _taskItemsRepository.AddTaskItem(taskItem);

            // 案件と作業の関連を追加する
            var projectTaskItem = new ProjectTaskItem
            {
                CompanyProjectId = selectedProject.CompanyProjectId,
                TaskItemId = newTaskItem.TaskItemId,
                IsCurrent = false,
                CreatedAt = DateTime.Now
            };
            _projectTaskItemsRepository.AddProjectTaskItem(projectTaskItem);

            // 選択された案件に関連する作業のリストを更新する
            LoadProjectTaskItems(selectedProject.CompanyProjectId);
            TaskItemName.Text = "";
        }

        /// <summary>
        /// アニメーションでボーダーを表示するメソッド
        /// </summary>
        /// <param name="borders"></param>
        private void ShowBorders(params Border[] borders)
        {
            foreach (var border in borders)
            {
                border.Visibility = Visibility.Visible;
                border.Opacity = 0;

                var transform = new TranslateTransform();
                border.RenderTransform = transform;

                var easing = new CubicEase
                {
                    EasingMode = EasingMode.EaseOut
                };

                var fadeAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromMilliseconds(1000),
                    EasingFunction = easing
                };

                var moveAnimation = new DoubleAnimation
                {
                    From = -10,
                    To = 0,
                    Duration = TimeSpan.FromMilliseconds(800),
                    EasingFunction = easing
                };

                border.BeginAnimation(
                    UIElement.OpacityProperty,
                    fadeAnimation);

                transform.BeginAnimation(
                    TranslateTransform.YProperty,
                    moveAnimation);
            }
        }

        /// <summary>
        /// 適用するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectSaveBtn_Click(object sender, RoutedEventArgs e)
        {
            var projectName = ProjectReNameBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show("案件名を入力してください。");
                return;
            }

            // 変更された案件名のIDを取得する
            if (ProjectReNameBox.Tag is not int projectId)
            {
                return;
            }

            // 変更された案件のメモのIDを取得する
            if (ProjectsMemoPanel.Tag is not int companyProjectId)
            {
                return;
            }

            // 案件情報を更新する
            var rslt = _projectManagement.ProjectUpdate(
                projectId,
                projectName,
                CompletedBox.IsChecked ?? false,
                companyProjectId,
                ProjectsMemoPanel.Text.Trim()
                );

            if(rslt)
            {
                MessageBox.Show("案件情報を更新しました。");
                LoadProjects();
            }
            else
            {
                MessageBox.Show("案件情報の更新に失敗しました。");
            }
        }

    }
}
