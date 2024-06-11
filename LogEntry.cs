using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ApacheLogs
{
    internal class LogEntry
    {
        public string Host { get; set; }
        public string Identity { get; set; }
        public string User { get; set; }
        public DateTime Timestamp { get; set; }
        public string Request { get; set; }
        public int Status { get; set; }
        public long Bytes { get; set; }

        public static LogEntry Parse(string logLine, string format)
        {
            var formatRegex = CreateRegexFromFormat(format);
            var match = formatRegex.Match(logLine);
            if (!match.Success)
            {
                throw new FormatException("Log line does not match format");
            }

            var logEntry = new LogEntry
            {
                Host = match.Groups["h"].Value,
                Identity = match.Groups["l"].Value,
                User = match.Groups["u"].Value,
                Timestamp = DateTime.ParseExact(match.Groups["t"].Value, "dd/MMM/yyyy:HH:mm:ss zzz", CultureInfo.InvariantCulture),
                Request = match.Groups["r"].Value,
                Status = int.Parse(match.Groups["s"].Value),
                Bytes = match.Groups["b"].Value == "-" ? 0 : long.Parse(match.Groups["b"].Value)
            };

            return logEntry;
        }

        private static Regex CreateRegexFromFormat(string format)
        {
            format = Regex.Escape(format)
                .Replace("%h", "(?<h>\\S+)")
                .Replace("%l", "(?<l>\\S*)")
                .Replace("%u", "(?<u>\\S*)")
                .Replace("%t", "\\[(?<t>[^\\]]+)\\]")
                .Replace("%r", "\"(?<r>[^\"]*)\"")
                .Replace("%>s", "(?<s>\\d{3})")
                .Replace("%b", "(?<b>\\S+|\\-)");

            return new Regex($"^{format}$");
        }


        public override string ToString()
        {
            return $"{Host} {Identity} {User} {Timestamp} \"{Request}\" {Status} {Bytes}";
        }
    }
}
