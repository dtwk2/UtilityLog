using System;
using System.Linq;

namespace Utility.Log.Model
{
    public class LogGroup : Utility.Log.Model.Log, IComparable<LogGroup>
    {
        public LogGroup(Guid key, Utility.Log.Model.Log[] logs)
        {
            this.Logs = logs;
            var first = logs.First();
            this.Details = first.Details;
            this.Date = first.Date;
            this.Level = first.Level;
            this.RunCount = logs.First().RunCount;
            this.Key = key;
        }

        public Utility.Log.Model.Log[] Logs { get; }

        public int CompareTo(LogGroup other)
        {
            return this.Date.CompareTo(other.Date);
        }
    }
}