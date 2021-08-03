using Splat;
using System;
using System.ComponentModel;

namespace Pcs.Hfrr.Log
{
    public class Log
    {
        public Log(string message, LogLevel level = LogLevel.Info, int runCount = 0)
        {
            Details = message;
            Level = level;
            Date = DateTime.Now;
            RunCount = runCount;
        }

        public Log(Exception exception, LogLevel level = LogLevel.Error, int runCount = 0)
        {
            Details = Newtonsoft.Json.JsonConvert.SerializeObject(exception);
            Level = level;
            Date = DateTime.Now;
            RunCount = runCount;
        }

        public Log()
        {
        }

        public Guid Key { get; set; }

        [Browsable(false)]
        public string Details { get; set; }

        public DateTime Date { get; set; }

        public LogLevel Level { get; set; }

        public int RunCount { get; set; }

        [Browsable(false)]
        [SQLite.Ignore]
        public Exception Exception => Deserialise();

        [SQLite.Ignore]
        public string Source => Exception?.Source;

        [SQLite.Ignore]
        public string Message => Exception?.Message ?? Details;

        [SQLite.Ignore]
        public string StackTrace => Exception?.StackTrace;

        private Exception Deserialise()
        {
            (Exception failure, bool b) = JsonHelper.TryParseJson<Exception>(Details);
            if (b)
                return failure;
            else
                return default;
        }
    }

    internal static class JsonHelper
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