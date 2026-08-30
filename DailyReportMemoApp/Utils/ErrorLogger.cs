using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReportMemoApp.Utils
{
    internal class ErrorLogger
    {
        public static void Write(Exception ex)
        {
            try
            {
                string appDataPath =
                    Environment.GetFolderPath(
                        Environment.SpecialFolder.LocalApplicationData);

                string logDirectory = Path.Combine(
                    appDataPath,
                    "ShigotoLog",
                    "Logs");

                Directory.CreateDirectory(logDirectory);

                string logFilePath = Path.Combine(
                    logDirectory,
                    $"error_{DateTime.Now:yyyyMMdd}.log");

                File.AppendAllText(
                    logFilePath,
                    $"""
                    ========================================
                    日時: {DateTime.Now:yyyy/MM/dd HH:mm:ss}
                    例外: {ex.GetType().FullName}
                    メッセージ: {ex.Message}

                    スタックトレース:
                    {ex.StackTrace}

                    """,
                    Encoding.UTF8);
            }
            catch
            {
                // ログ出力失敗によってアプリを落とさない
            }
        }
    }
}
