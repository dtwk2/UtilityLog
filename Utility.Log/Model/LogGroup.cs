using System;
using System.Linq;

namespace Pcs.Hfrr.Log.View.Infrastructure
{
    public class LogGroup : Log, IComparable<LogGroup>
    {
        public LogGroup(Guid key, Log[] logs)
        {
            this.Logs = logs;
            var first = logs.First();
            this.Details = first.Details;
            this.Date = first.Date;
            this.Level = first.Level;
            this.RunCount = logs.First().RunCount;
            this.Key = key;
        }

        public Log[] Logs { get; }

        public int CompareTo(LogGroup other)
        {
            return this.Date.CompareTo(other.Date);
        }
    }
}