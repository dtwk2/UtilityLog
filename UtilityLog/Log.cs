using System;
using Splat;

namespace UtilityLog
{
    public class Log
    {
        public Log(string message, LogLevel level, int runCount = 0)
        {
            Message = message;
            Level = level;
            Date = DateTime.Now;
            RunCount = runCount;
        }

        public Log()
        {
        }

        public DateTime Date { get; set; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public int RunCount { get; set; }
   }
}