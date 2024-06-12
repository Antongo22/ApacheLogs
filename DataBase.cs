using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace ApacheLogs
{
    internal static class DataBase
    {
        private static readonly string DatabasePath = "logs.db";

        public static void Create()
        {
            if (File.Exists(DatabasePath))
            {
                File.Delete(DatabasePath);
            }

            SQLiteConnection.CreateFile(DatabasePath);

            using (var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;"))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE Logs (
                        ip TEXT,
                        dateofrequest DATETIME,
                        request TEXT,
                        status INTEGER
                    )";

                ExecuteNonQuery(connection, createTableQuery);
            }
        }

        private static void InsertLog(LogEntry logEntry)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;"))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO Logs (ip, dateofrequest, request, status)
                    VALUES (@ip, @dateofrequest, @request, @status)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ip", logEntry.Data["%h"]);
                    command.Parameters.AddWithValue("@dateofrequest", DateTime.Parse(logEntry.Data["%t"]));
                    command.Parameters.AddWithValue("@request", logEntry.Data["%r"]);
                    command.Parameters.AddWithValue("@status", int.Parse(logEntry.Data["%>s"]));

                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool SetDatas(List<LogEntry> logs)
        {
            bool isSuccess = true;

            foreach (LogEntry logEntry in logs)
            {
                if (logEntry.Data.ContainsKey("%h") && logEntry.Data.ContainsKey("%t") && logEntry.Data.ContainsKey("%r") && logEntry.Data.ContainsKey("%>s"))
                {
                    try
                    {
                        InsertLog(logEntry);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Ошибка при вносе данных: {ex.Message}");
                        isSuccess = false;
                    }
                }
                else
                {
                    Console.WriteLine("Ошибка: недостаточно данных для вставки логов.");
                    isSuccess = false;
                }
            }

            return isSuccess;
        }

        public static void GetLogs()
        {
            GetLogsByFilter(null, null, null, null);
        }

        public static void GetLogsByFilter(DateTime? dateFrom, DateTime? dateTo, string ip, int? status)
        {
            using (var connection = new SQLiteConnection($"Data Source={DatabasePath};Version=3;"))
            {
                connection.Open();

                var selectQuery = new List<string>
                {
                    "SELECT ip, dateofrequest, request, status FROM Logs WHERE 1=1"
                };

                if (dateFrom.HasValue)
                {
                    selectQuery.Add($" AND dateofrequest >= '{dateFrom.Value:yyyy-MM-dd}'");
                }

                if (dateTo.HasValue)
                {
                    selectQuery.Add($" AND dateofrequest <= '{dateTo.Value:yyyy-MM-dd}'");
                }

                if (!string.IsNullOrEmpty(ip))
                {
                    selectQuery.Add($" AND ip = '{ip}'");
                }

                if (status.HasValue)
                {
                    selectQuery.Add($" AND status = {status}");
                }

                using (var command = new SQLiteCommand(string.Join(" ", selectQuery), connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string logIp = reader.GetString(0);
                        DateTime logDateOfRequest = reader.GetDateTime(1);
                        string logRequest = reader.GetString(2);
                        int logStatus = reader.GetInt32(3);

                        Console.WriteLine($"IP: {logIp}, Date: {logDateOfRequest}, Request: {logRequest}, Status: {logStatus}");
                    }
                }
            }
        }

        private static void ExecuteNonQuery(SQLiteConnection connection, string query)
        {
            using (var command = new SQLiteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
