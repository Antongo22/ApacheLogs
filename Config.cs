using System;
using System.Collections.Generic;
using System.IO;

namespace ApacheLogs
{
    internal class Config
    {
        public string FilesDir { get; set; }
        public string Ext { get; set; }
        public string Format { get; set; }
        public int MinuteOfUpdate { get; set; }

        public static Config LoadFromFile(string configPath)
        {
            if (!File.Exists(configPath))
            {
                Console.WriteLine("Файл конфигурации не найден!");
                return null;
            }

            var config = new Config();
            var lines = File.ReadAllLines(configPath);

            foreach (var line in lines)
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim().ToLower();
                var value = parts[1].Trim();

                switch (key)
                {
                    case "files_dir":
                        config.FilesDir = value;
                        break;
                    case "ext":
                        config.Ext = value;
                        break;
                    case "format":
                        config.Format = value;
                        break;
                    case "time":
                        if (!int.TryParse(value, out var time))
                        {
                            Console.WriteLine($"Ошибка: неверный формат времени обновления '{value}', установлено значение по умолчанию 60 минут.");
                            time = 60;
                        }
                        config.MinuteOfUpdate = time;
                        break;
                }
            }

            if (string.IsNullOrEmpty(config.FilesDir) || string.IsNullOrEmpty(config.Ext) || string.IsNullOrEmpty(config.Format))
            {
                Console.WriteLine("Ошибка: не все необходимые параметры заданы в конфигурационном файле.");
                return null;
            }

            return config;
        }
    }
}
