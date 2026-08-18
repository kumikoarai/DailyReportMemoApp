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

        public ProjectManagementPage(int companyId)
        {
            InitializeComponent();

            //var TTLText = new TextBlock
            //{
            //    Margin = new Thickness(0, 0, 0, 20),
            //    Text = $"テストととと: {companyId}",
            //    VerticalAlignment = VerticalAlignment.Center,
            //    TextWrapping = TextWrapping.Wrap,
            //    FontSize = 16,
            //    FontWeight = FontWeights.Bold
            //};

            //ProjectsPanel.Children.Add(TTLText);

            LoadProjects(companyId);
        }

        /// <summary>
        /// 会社に関連する案件のリストを取得してDataGridにバインドするメソッド
        /// </summary>
        private void LoadProjects(int companyId)
        {
            // 選択された会社に関連する案件のリストを取得してDataGridにバインドする
            ProjectsListBox.ItemsSource = _companyProjectsRepository.GetCompanyProjects(companyId);
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

                return;
            }

            // 選択された案件に関連する作業のリストを取得してDataGridにバインドする
            LoadProjectTaskItems(selectedProject.CompanyProjectId);

            // 作業のリストを有効化する
            TaskItemsListBox.IsEnabled = true;
            TaskItemName.IsEnabled = true;
            TaskItemNameBtn.IsEnabled = true;
        }


        /// <summary>
        /// 案件名を入力して追加するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProjectEntry_Click(object sender, RoutedEventArgs e)
        {
            //var projectName = ProjectName.Text.Trim();

            //if (string.IsNullOrWhiteSpace(projectName))
            //{
            //    MessageBox.Show("案件名を入力してください。");
            //    return;
            //}


            //// 選択された会社を取得する
            //var selectedCompany = CompaniesListBox.SelectedItem as Company;
            //if (selectedCompany == null)
            //{
            //    MessageBox.Show("会社を選択してください。");
            //    return;
            //}
            //else
            //{

            //    // 会社に関連する案件を追加する
            //    var project = new Project
            //    {
            //        ProjectName = projectName,
            //        Completed = false,
            //        CreatedAt = DateTime.Now
            //    };
            //    var newProject = _projectsRepository.AddProject(project);

            //    // 会社と案件の関連を追加する
            //    var companyProject = new CompanyProject
            //    {
            //        CompanyId = selectedCompany.CompanyId,
            //        ProjectId = newProject.ProjectId,
            //        CreatedAt = DateTime.Now
            //    };
            //    _companyProjectsRepository.AddCompanyProject(companyProject);
            //}

            //LoadCompanyProjects(selectedCompany.CompanyId);
            //ProjectName.Text = "";
        }


        /// <summary>
        /// 作業名を入力して追加するボタンがクリックされたときの処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TaskItemEntry_Click(object sender, RoutedEventArgs e)
        {
            //var taskItemName = TaskItemName.Text.Trim();

            //if (string.IsNullOrWhiteSpace(taskItemName))
            //{
            //    MessageBox.Show("作業名を入力してください。");
            //    return;
            //}

            //var selectedCompany = CompaniesListBox.SelectedItem as Company;
            //var selectedProject = ProjectsListBox.SelectedItem as CompanyProject;

            //if (selectedCompany == null)
            //{
            //    MessageBox.Show("会社を選択してください。");
            //    return;
            //}

            //if (selectedProject == null)
            //{
            //    MessageBox.Show("案件を選択してください。");
            //    return;
            //}

            //// 選択された案件に関連する作業を追加する
            //var taskItem = new TaskItem
            //{
            //    TaskItemName = taskItemName,
            //    CreatedAt = DateTime.Now
            //};
            //var newTaskItem = _taskItemsRepository.AddTaskItem(taskItem);

            //// 案件と作業の関連を追加する
            //var projectTaskItem = new ProjectTaskItem
            //{
            //    CompanyProjectId = selectedProject.CompanyProjectId,
            //    TaskItemId = newTaskItem.TaskItemId,
            //    IsCurrent = false,
            //    CreatedAt = DateTime.Now
            //};
            //_projectTaskItemsRepository.AddProjectTaskItem(projectTaskItem);

            //// 選択された案件に関連する作業のリストを更新する
            //LoadProjectTaskItems(selectedProject.CompanyProjectId);
            //TaskItemName.Text = "";
        }

    }
}
