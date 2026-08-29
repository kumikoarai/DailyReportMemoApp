using System;
using System.IO;

namespace DailyReportMemoApp.Common
{
    public static class AppPaths
    {
        public static string DataFolderPath =>
            Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ShigotoLog"
            );

        public static string DatabasePath =>
            Path.Combine(
                DataFolderPath,
                "ShigotoLog.db"
            );

    }
}
