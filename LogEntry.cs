using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ApacheLogs
{
    internal class LogEntry
    {
        private LogEntry(Dictionary<string, string> data)
        {
            Data = data;
        }

        public Dictionary<string, string> Data { get; }

        public static LogEntry Parse(string logLine, string format)
        {
            var splits = SplitText(logLine);
            var formats = format.Split(' ').ToList();

            return new LogEntry(GetData(splits, formats));
        }

        private static List<string> SplitText(string input)
        {
            return Regex.Matches(input, @"(?:^|\s)(?:""[^""]*""|\[[^\]]*\]|[^\s]+)(?=\s|$)")
                        .Cast<Match>()
                        .Select(m => m.Value.Trim())
                        .ToList();
        }

        private static Dictionary<string, string> GetData(List<string> splits, List<string> formats)
        {
            if (splits.Count != formats.Count)
                throw new Exception("Log entries do not match the format! Parameter count mismatch.");

            var data = new Dictionary<string, string>();

            for (int i = 0; i < formats.Count; i++)
            {
                switch (formats[i])
                {
                    case "%t" when splits[i].StartsWith("["):
                        string dateTimeStr = splits[i].Trim('[', ']');
                        string format = "dd/MMM/yyyy:HH:mm:ss zzz";
                        if (DateTime.TryParseExact(dateTimeStr, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
                        {
                            data.Add(formats[i], dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            throw new Exception("Log entries do not match the format! Error processing timestamp.");
                        }
                        break;

                    case "%r" when splits[i].StartsWith("\""):
                        data.Add(formats[i], splits[i].Trim('\"'));
                        break;

                    default:
                        data.Add(formats[i], splits[i]);
                        break;
                }
            }

            return data;
        }

        public override string ToString()
        {
            return string.Join(" ", Data.Select(d => d.ToString()));
        }
    }
}
