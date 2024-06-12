using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApacheLogs
{
    internal class Program
    {
        static List<LogEntry> logs;
        static void Main(string[] args)
        {
            DataBase.Create();
            Console.WriteLine("Database created successfully.");

            Parse();

            bool isSuc = DataBase.SetDatas(logs);
            Console.WriteLine("Log inserted successfully.");

            DataBase.GetLogs();
        }


        static void Parse()
        {
            List<LogEntry> res = Apache.Parse("config.txt");
            if(res == null)
            {
                Console.WriteLine("Не удалось считать днные");
            }
            else
            {
                logs = res;
            }
        }
    }
}
