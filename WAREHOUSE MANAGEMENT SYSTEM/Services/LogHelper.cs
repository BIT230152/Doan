using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Services
{
    public class LogHelper
    {
        private static readonly string logFilePath = "Logs/HistoryLog.txt";

        public static void WriteLog(string action, Guid productId, string productName, int Count)
        {
            try
            {
                string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {action} | ID: {productId} | Sản phẩm: {productName} | Số lượng: {Count}";

                // Đảm bảo thư mục Logs tồn tại
                string directory = Path.GetDirectoryName(logFilePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                // Ghi vào file
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi ghi lịch sử: " + ex.Message);
            }
        }
        public static List<string> ReadLogs()
        {
            List<string> logs = new List<string>();

            if (File.Exists(logFilePath))
            {
                logs = File.ReadAllLines(logFilePath).ToList();
            }

            return logs;
        }
    }
}
