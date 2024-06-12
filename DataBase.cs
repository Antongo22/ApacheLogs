using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

namespace ApacheLogs
{
    internal static class DataBase
    {
        static string databasePath = "logs.db";

        public static void Create()
        {
            if (File.Exists(databasePath))
            {
                File.Delete(databasePath);
            }

            SQLiteConnection.CreateFile(databasePath);

            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                string createTableQuery = @"
                    CREATE TABLE Logs (
                        ip TEXT,
                        dateofrequest DATETIME,
                        request TEXT,
                        status INTEGER
                    )";

                using (var command = new SQLiteCommand(createTableQuery, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        private static void InsertLog(string ip, DateTime dateOfRequest, string request, int status)
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                string insertQuery = @"
                    INSERT INTO Logs (ip, dateofrequest, request, status)
                    VALUES (@ip, @dateofrequest, @request, @status)";

                using (var command = new SQLiteCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@ip", ip);
                    command.Parameters.AddWithValue("@dateofrequest", dateOfRequest);
                    command.Parameters.AddWithValue("@request", request);
                    command.Parameters.AddWithValue("@status", status);

                    command.ExecuteNonQuery();
                }
            }
        }

        public static bool SetDatas(List<LogEntry> res)
        {
            bool isSuc = false;
            foreach (LogEntry logEntry in res)
            {
                if (logEntry.Data.ContainsKey("%h") && logEntry.Data.ContainsKey("%t") && logEntry.Data.ContainsKey("%r") && logEntry.Data.ContainsKey("%>s"))
                {
                    isSuc = true;
                    InsertLog(logEntry.Data["%h"], DateTime.Parse(logEntry.Data["%t"]), logEntry.Data["%r"], int.Parse(logEntry.Data["%>s"]));
                }
                else
                {
                    Console.WriteLine("Ошибка при вносе данных!");
                }
            }

            return isSuc;
        }

        public static void GetLogs()
        {
            using (var connection = new SQLiteConnection($"Data Source={databasePath};Version=3;"))
            {
                connection.Open();

                string selectQuery = "SELECT ip, dateofrequest, request, status FROM Logs";

                using (var command = new SQLiteCommand(selectQuery, connection))
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string ip = reader.GetString(0);
                        DateTime dateOfRequest = reader.GetDateTime(1);
                        string request = reader.GetString(2);
                        int status = reader.GetInt32(3);

                        Console.WriteLine($"IP: {ip}, Date: {dateOfRequest}, Request: {request}, Status: {status}");
                    }
                }
            }
        }
    }
}
