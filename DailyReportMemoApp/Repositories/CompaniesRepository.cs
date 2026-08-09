using DailyReportMemoApp.Data;
using DailyReportMemoApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Repositories
{
    public class CompaniesRepository
    {
        /// <summary>
        /// 会社のリストを取得するメソッド
        /// </summary>
        /// <returns>会社リストを返す</returns>
        public List<Company> GetAllCompanies()
        {
            using (var db = new AppDbContext())
            {
                return db.Companies
                       .ToList();
            };

        }

        /// <summary>
        /// 会社を追加するメソッド
        /// </summary>
        /// <param name="company"></param>
        public void AddCompany(Company company)
        {
            using (var db = new AppDbContext())
            {
                db.Companies.Add(company);
                db.SaveChanges();
            }
        }
    }
}
