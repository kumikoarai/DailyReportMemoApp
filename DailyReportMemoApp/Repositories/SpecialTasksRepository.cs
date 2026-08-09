using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class SpecialTasksRepository
    {
        /// <summary>
        /// デフォルト作業項目を取得する
        /// </summary>
        /// <returns></returns>
        public List<SpecialTask> GetSpecialTasks()
        {
            using (var db = new Data.AppDbContext())
            {
                return db.SpecialTasks
                         .Where(x => !x.IsDeleted && x.IsActive)
                         .ToList();
            }
        }

        /// <summary>
        /// 特別作業のデフォルト開始フラグの作業を取得する
        /// </summary>
        /// <returns></returns>
        public SpecialTask? GetDefaulSpecialTasks()
        {
            using (var db = new Data.AppDbContext())
            {
                return db.SpecialTasks
                         .FirstOrDefault(x => x.DefaultStartFlg);
            }
        }

    }
}
