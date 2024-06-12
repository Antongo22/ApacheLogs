﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace ApacheLogs
{
    internal class Program
    {
        static string configpath = "config.txt";

        static void Main(string[] args)
        {
            ShowWelcomeMessage();

            while (true)
            {
                Console.Write("> ");

                string command = Console.ReadLine();

                switch (command.ToLower().Trim())
                {
                    case "parse":
                        Parse(configpath);
                        break;
                    case "openconfig":
                        OpenFileInDefaultProgram(configpath);
                        break;
                    case var s when s.StartsWith("getlog"):
                        GetData(s);
                        break;
                    case "clear":
                        ClearConsole();
                        break;
                    case "close":
                        return;
                    default:
                        ConsoleHelper.WriteError("Неизвестная команда!");
                        break;
                }
            }
        }

        static void ShowWelcomeMessage()
        {
            ConsoleHelper.WriteInfo("Добро пожаловать в программу по просмотрк логов. Вот список доступных команд\n" +
                              "openconfig - открывает в редакторе по умолчанию файл конфига\n" +
                              "close - завершает выполнение программы\n" +
                              "parse - получает данные из конфига, сопоставляет их с логими. Полученные данные из логов записывает в базу данных\n" +
                              "getlog (date|datefrom) (dateto) (ip) (status) - получает данные логов из уже выгруженной базе данных.\n" +
                              "clear - очищает консоль и выводит доступные команды.");
        }

        static void OpenFileInDefaultProgram(string filePath)
        {
            try
            {
                Process.Start(filePath);
                ConsoleHelper.WriteInfo("Не забудьте сохранить файл и прописать команду parse, если это требуется!");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError("Ошибка при открытии файла: " + ex.Message);
            }
        }

        static void Parse(string configpath)
        {
            var logs = Apache.Parse(configpath);
            if (logs == null)
            {
                ConsoleHelper.WriteError("Не удалось считать данные");
            }
            else
            {
                DataBase.Create();
                bool isSuccess = DataBase.SetDatas(logs);
                if (isSuccess)
                {
                    ConsoleHelper.WriteInfo("Данные успешно получены и записаны в базу данных!");
                }
                else
                {
                    ConsoleHelper.WriteError("Произошли ошибки при записи данных в базу.");
                }
            }
        }

        static void GetData(string commandline)
        {
            string[] commands = commandline.Trim().Split(' ');

            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            string ip = null;
            int? status = null;

            for (int i = 1; i < commands.Length; i++)
            {
                string datetmp = commands[i];
                DateTime date;

                if (int.TryParse(datetmp, out int sta))
                {
                    status = sta;
                }
                else if (datetmp.Count(c => c == '.') == 3)
                {
                    ip = datetmp;
                }
                else if (DateTime.TryParseExact(datetmp, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                        DateTime.TryParseExact(datetmp, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                        DateTime.TryParseExact(datetmp, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                        DateTime.TryParseExact(datetmp, "dd/MMM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
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
                    ConsoleHelper.WriteError("Неверный формат данных!");
                    return;
                }
            }

            DataBase.GetLogsByFilter(dateFrom, dateTo, ip, status);
        }

        static void ClearConsole()
        {
            Console.Clear();
            ShowWelcomeMessage();
        }
    }
}
