using System;
using System.ComponentModel;
using Splat;

namespace UtilityLog
{
    public class Log
    {
 
        public Log(string message, LogLevel level = LogLevel.Info, int runCount = 0)
        {
            ExceptionText = message;
            Level = level;
            Date = DateTime.Now;
            RunCount = runCount;
        }

        public Log(Exception exception, LogLevel level = LogLevel.Error, int runCount = 0)
        {
            ExceptionText = Newtonsoft.Json.JsonConvert.SerializeObject(new LogException { Source = exception.Source, Message = exception.Message, StackTrace = exception.StackTrace });
            Level = level;
            Date = DateTime.Now;
            RunCount = runCount;
        }

        public Log()
        {
        }

        public Guid Key { get; set; }

        public string ExceptionText { get; set; }

        public DateTime Date { get; set; }

        public LogLevel Level { get; set; }

        public int RunCount { get; set; }

        [Browsable(false)]
        [SQLite.Ignore]
        public LogException Exception => Deserialise();

        [SQLite.Ignore]
        public string Source => Exception.Source;

        [SQLite.Ignore]
        public string Message => Exception.Message;

        [SQLite.Ignore]
        public string StackTrace => Exception.StackTrace;

        LogException Deserialise()
        {
            (LogException failure, bool b) = JsonHelper.TryParseJson<LogException>(ExceptionText);
            if (b)
                return failure;
            else
                return default;
        }
   }

    public struct LogException
    {

        public string Source { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        //public LogException InnerException { get; set; }

    }

    static class JsonHelper
    {
        public static (T value, bool isValid) TryParseJson<T>(string jsonString)
        {
            if (string.IsNullOrEmpty(jsonString) == true)
                return (default, default);

            jsonString = jsonString.Trim();
            if ((jsonString.StartsWith("{") && jsonString.EndsWith("}")) || //For object
                (jsonString.StartsWith("[") && jsonString.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                    return (obj, true);
                }
                catch (Newtonsoft.Json.JsonReaderException jex)
                {
                    //Exception in parsing json
                    //Console.WriteLine(jex.Message);
                    return (default, default);
                }
                catch (Exception ex) //some other exception
                {
                    //Console.WriteLine(ex.ToString());
                    return (default, default);
                }
            }
            else
            {
                return (default, default);
            }
        }
    }
}