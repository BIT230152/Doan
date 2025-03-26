using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Services
{
    public static class InventoryLogHelper
    {
        private const string LogPath = "Logs/InventoryLog.txt";

        public class InventoryLogEntry
        {
            public string Time { get; set; }
            public string Action { get; set; }
            public string Product { get; set; }
            public string Quantity { get; set; }
        }

        public static void WriteLog(string action, string productName, int quantity)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(LogPath));
                File.AppendAllText(LogPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|{action}|{productName}|{quantity}\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi ghi log: {ex.Message}");
            }
        }

        public static List<InventoryLogEntry> ReadAllLogs()
        {
            var result = new List<InventoryLogEntry>();

            if (!File.Exists(LogPath))
                return result;

            foreach (var line in File.ReadAllLines(LogPath))
            {
                var parts = line.Split('|');
                if (parts.Length >= 4)
                {
                    result.Add(new InventoryLogEntry
                    {
                        Time = parts[0],
                        Action = parts[1],
                        Product = parts[2],
                        Quantity = parts[3]
                    });
                }
            }

            return result.OrderByDescending(x => x.Time).ToList();
        }
    }
}