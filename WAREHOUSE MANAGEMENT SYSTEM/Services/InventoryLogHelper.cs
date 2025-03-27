using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WAREHOUSE_MANAGEMENT_SYSTEM.Data.Models;

namespace WAREHOUSE_MANAGEMENT_SYSTEM.Services
{
    public static class InventoryLogHelper
    {
        private const string LogDirectory = "Logs";
        private const string LogFileName = "InventoryHistory.txt";
        private static readonly string LogPath = Path.Combine(LogDirectory, LogFileName);

        /// <summary>
        /// Ghi log nhập/xuất kho vào file
        /// </summary>
        public static void LogMovement(StockMovement movement, Product product)
        {
            try
            {
                EnsureLogDirectoryExists();

                var logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}|" +
                              $"{(movement.MovementType == MovementType.Import ? "NHẬP" : "XUẤT")}|" +
                              $"{product.Id}|{product.Name}|" +
                              $"{movement.Quantity}|{movement.Country}";

                File.AppendAllText(LogPath, logEntry + Environment.NewLine);
            }
            catch (Exception ex)
            {
                // Ghi lỗi vào console hoặc hệ thống log tập trung
                Console.WriteLine($"Lỗi khi ghi log kho: {ex.Message}");
                // Có thể throw exception nếu cần thiết
            }
        }

        /// <summary>
        /// Đọc toàn bộ lịch sử nhập/xuất kho
        /// </summary>
        public static List<InventoryLogEntry> GetHistory()
        {
            try
            {
                if (!File.Exists(LogPath))
                    return new List<InventoryLogEntry>();

                return File.ReadAllLines(LogPath)
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .Select(ParseLogEntry)
                    .Where(entry => entry != null)
                    .OrderByDescending(x => x.Timestamp)
                    .ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc log kho: {ex.Message}");
                return new List<InventoryLogEntry>();
            }
        }

        /// <summary>
        /// Xóa toàn bộ lịch sử log
        /// </summary>
        //public static void ClearHistory()
        //{
        //    try
        //    {
        //        if (File.Exists(LogPath))
        //        {
        //            File.Delete(LogPath);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Lỗi khi xóa log kho: {ex.Message}");
        //    }
        //}

        private static void EnsureLogDirectoryExists()
        {
            if (!Directory.Exists(LogDirectory))
            {
                Directory.CreateDirectory(LogDirectory);
            }
        }

        private static InventoryLogEntry ParseLogEntry(string logLine)
        {
            try
            {
                var parts = logLine.Split('|');
                if (parts.Length != 6) return null;

                return new InventoryLogEntry
                {
                    Timestamp = parts[0],
                    Action = parts[1],
                    ProductId = Guid.Parse(parts[2]),
                    ProductName = parts[3],
                    Quantity = int.Parse(parts[4]),
                    Country = parts[5]
                };
            }
            catch
            {
                // Bỏ qua các dòng log không đúng định dạng
                return null;
            }
        }

        public class InventoryLogEntry
        {
            public string Timestamp { get; set; }
            public string Action { get; set; } // "NHẬP" hoặc "XUẤT"
            public Guid ProductId { get; set; }
            public string ProductName { get; set; }
            public int Quantity { get; set; }
            public string Country { get; set; }
        }
    }
}