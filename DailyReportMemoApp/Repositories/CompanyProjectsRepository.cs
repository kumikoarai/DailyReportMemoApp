using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using DailyReportMemoApp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DailyReportMemoApp.Repositories
{
    public class CompanyProjectsRepository
    {
        /// <summary>
        /// 会社案件のリストを取得するメソッド
        /// </summary>
        /// <returns></returns>
        public List<CompanyProject> GetCompanyProjects(int companyId)
        {
            using (var db = new AppDbContext())
            {
                return db.CompanyProjects
                       .Include(x => x.Companies)
                       .Include(x => x.Projects)
                       .Where(cp => cp.CompanyId == companyId)
                       .ToList();
            };
        }

        /// <summary>
        /// 会社案件を追加するメソッド
        /// </summary>
        /// <param name="companyProject"></param>
        public CompanyProject AddCompanyProject(CompanyProject companyProject)
        {
            using (var db = new AppDbContext())
            {
                db.CompanyProjects.Add(companyProject);
                db.SaveChanges();
            }
            return companyProject;
        }

        /// <summary>
        /// 会社案件のメモを更新
        /// </summary>
        /// <param name="workLog"></param>
        /// <returns></returns>
        public MemoSaveInfo? UpdateCompanyProjectMemo(MemoSaveInfo memoSaveInfo)
        {
            using (var db = new Data.AppDbContext())
            {
                if (memoSaveInfo.WorkLog.ProjectTaskItems == null ||
                    memoSaveInfo.WorkLog.ProjectTaskItems.CompanyProjects == null)
                {
                    return null;
                }

                var targetWorkLog = db.CompanyProjects
                    .FirstOrDefault(x => x.CompanyProjectId == memoSaveInfo.WorkLog.ProjectTaskItems.CompanyProjects.CompanyProjectId);

                if (targetWorkLog == null)
                {
                    MessageBox.Show("保存対象の作業データが見つかりませんでした。");
                    return null;
                }

                var now = DateTime.Now;

                targetWorkLog.Memo = memoSaveInfo.TextBox.Text;
                targetWorkLog.UpdatedAt = now;

                db.SaveChanges();

                memoSaveInfo.WorkLog.ProjectTaskItems.CompanyProjects.Memo = memoSaveInfo.TextBox.Text;
                memoSaveInfo.WorkLog.UpdatedAt = targetWorkLog.UpdatedAt;

                return memoSaveInfo;
            }
        }


    }
}
