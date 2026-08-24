using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class TaskItemsRepository
    {
        /// <summary>
        /// 作業項目のリストを取得するメソッド
        /// </summary>
        /// <returns></returns>
        public List<TaskItem> GetAllProjects()
        {
            using (var db = new AppDbContext())
            {
                return db.TaskItems
                       .ToList();
            };

        }

        /// <summary>
        /// 作業項目を追加するメソッド
        /// </summary>
        /// <param name="taskItem"></param>
        public TaskItem AddTaskItem(AppDbContext db, TaskItem taskItem)
        {
            db.TaskItems.Add(taskItem);
            db.SaveChanges();

            return taskItem;
        }
    }
}
