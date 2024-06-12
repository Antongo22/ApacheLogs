using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApacheLogs
{
    internal class Program
    {
        static readonly string ConfigPath = "config.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в программу по просмотрк логов. Вот список доступных команд\n" +
                              "openconfig - открывает в редакторе по умолчанию файл конфига\n" +
                              "close - завершает выполнение программы в том числе tryapp\n" +
                              "parse - получает данные из конфига, сопоставляет их с логами. Полученные данные из логов записывает в базу данных\n" +
                              "getlog (date|datefrom) (dateto) (ip) (status) - получает данные логов из уже выгруженной базе данных.");

            while (true)
            {
                Console.Write("> ");
                string command = Console.ReadLine()?.Trim().ToLower();

                switch (command)
                {
                    case "parse":
                        Parse(ConfigPath);
                        break;
                    case "openconfig":
                        OpenFileInDefaultProgram(ConfigPath);
                        break;
                    case string s when s.StartsWith("getlog"):
                        GetData(s);
                        break;
                    case "close":
                        return;
                    default:
                        Console.WriteLine("Неизвестная команда!");
                        break;
                }
            }
        }

        static void OpenFileInDefaultProgram(string filePath)
        {
            try
            {
                Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                Console.WriteLine("Не забудьте сохранить файл и прописать команду parse, если это требуется!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
            }
        }

        static void Parse(string configPath)
        {
            List<LogEntry> logEntries = Apache.Parse(configPath);
            if (logEntries == null)
            {
                Console.WriteLine("Не удалось считать данные");
            }
            else
            {
                if (DataBase.SetDatas(logEntries))
                {
                    Console.WriteLine("Данные успешно получены и записаны в базу данных!");
                }
                else
                {
                    Console.WriteLine("Ошибка при записи данных в базу");
                }
            }
        }

        static void GetData(string commandLine)
        {
            string[] commands = commandLine.Split(' ');

            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            string ip = null;
            int? status = null;

            for (int i = 1; i < commands.Length; i++)
            {
                string param = commands[i];

                if (int.TryParse(param, out int sta) && param.Length == 3)
                {
                    status = sta;
                }
                else if (Regex.IsMatch(param, @"^\d{1,3}(\.\d{1,3}){3}$"))
                {
                    ip = param;
                }
                else if (DateTime.TryParseExact(param, new[] { "dd-MM-yyyy", "dd/MM/yyyy", "dd.MM.yyyy", "dd/MMM/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    if (dateFrom == null)
                    {
                        dateFrom = date;
                    }
                    else
                    {
                        dateTo = date;
                    }
                }
                else
                {
                    Console.WriteLine("Неверный формат данных!");
                    return;
                }
            }

            DataBase.GetLogsByFilter(dateFrom, dateTo, ip, status);
        }
    }
}
