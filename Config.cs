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
            var config = new Config();
            var lines = File.ReadAllLines(configPath);
            foreach (var line in lines)
            {
                var parts = line.Split(new[] { '=' }, 2);
                if (parts.Length != 2) continue;

                var key = parts[0].Trim();
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
                        if (int.TryParse(value, out var time))
                        {
                            config.MinuteOfUpdate = time;
                        }
                        else
                        {
                            config.MinuteOfUpdate = 60;
                        }
                        break;
                }
            }

            if (config.FilesDir == null || config.Ext == null || config.Format == null || config.MinuteOfUpdate == 0)
            {
                return null;
            }

            return config;
        }
    }
}
