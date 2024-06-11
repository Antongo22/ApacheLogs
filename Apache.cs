using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ApacheLogs
{
    internal class Apache
    {
        public static void Parse(string configFile)
        {
            try
            {
               
                var config = Config.LoadFromFile(configFile);
                var logFiles = Directory.GetFiles(config.FilesDir, $"*.{config.Ext}");

                if (logFiles == null || logFiles.Length == 0)
                {
                    throw new Exception("В выбранной ввами папке нет файлов с данным расширением!");
                }

                foreach (var logFile in logFiles)
                {
                    var logLines = File.ReadAllLines(logFile);
                    foreach (var logLine in logLines)
                    {
                        try
                        {
                            Console.WriteLine(logLine);
                            var logEntry = LogEntry.Parse(logLine, config.Format);
                            Console.WriteLine(logEntry);
                        }
                        catch (FormatException e)
                        {
                            throw new Exception($"Ошибка при попытке обработки данных: {e.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

}
