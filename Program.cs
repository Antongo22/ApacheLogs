using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ApacheLogs
{
    internal class Program
    {
        static string configpath = "config.txt";

        static void Main(string[] args)
        {
            Console.WriteLine("Добро пожаловать в программу по просмотрк логов. Вот список доступных команд\n" +
                              "openconfig - открывает в редакторе по умолчанию файл конфига\n" +
                              "close - завершает выполнение программы\n" +
                              "parse - получает данные из конфига, сопоставляет их с логими. Полученные данные из логов записывает в базу данных\n" +
                              "getlog (date|datefrom) (dateto) (ip) (status) - получает данные логов из уже выгруженной базе данных.");

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
                Process.Start(filePath);
                Console.WriteLine("Не забудьте сохранить файл и прописать команду parse, если это требуется!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при открытии файла: " + ex.Message);
            }
        }

        static void Parse(string configpath)
        {
            List<LogEntry> res = Apache.Parse(configpath);
            if(res == null)
            {
                Console.WriteLine("Не удалось считать днные");
            }
            else
            {
                SetDateBase(res);
                Console.WriteLine("Данные успешно получены!");
            }
        }

        static void SetDateBase(List<LogEntry> logs)
        {
            DataBase.Create();
            bool isSuc = DataBase.SetDatas(logs);
        }

        static void GetData(string commandline)
        {
            string[] commands = commandline.Trim().Split(' ');

            Console.WriteLine(commands.Length);

            DateTime? dateFrom = null;
            DateTime? dateTo = null;
            string ip = null;
            int? status = null;


            for (int i = 1; i < commands.Length; i++)
            {
                string datetmp = commands[i];
                DateTime date;

                Console.WriteLine(datetmp);
                if (datetmp.Length == 3 && int.TryParse(datetmp, out int sta))
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
                        DateTime.TryParseExact(datetmp, "dd/MMM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
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
                    Console.WriteLine("Неверный формат данных!4");
                    return;
                }
            }
               
            DataBase.GetLogsByFilter(dateFrom, dateTo, ip, status);          
        }
    }
}
