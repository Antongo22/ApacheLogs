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
        public static List<LogEntry> Parse(string configFile)
        {
            try
            {           
                var config = Config.LoadFromFile(configFile);

                if(config == null)
                {
                    throw new Exception("Неправильно задан файл конфигурации!");
                }

                var logFiles = Directory.GetFiles(config.FilesDir, $"*.{config.Ext}");

                if (logFiles == null || logFiles.Length == 0)
                {
                    throw new Exception("В выбранной ввами папке нет файлов с данным расширением!");
                }

                List<LogEntry> result = new List<LogEntry>();

                foreach (var logFile in logFiles)
                {
                    var logLines = File.ReadAllLines(logFile);
                    foreach (var logLine in logLines)
                    {
                        try
                        {
                            var logEntry =  LogEntry.Parse(logLine, config.Format);
                            result.Add(logEntry);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Ошибка при попытке обработки данных: {e.Message}");
                        }
                    }
                }
                if(result.Count == 0)
                {
                    return null;
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }

}
