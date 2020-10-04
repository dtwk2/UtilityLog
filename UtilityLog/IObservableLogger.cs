using Splat;
using System;

namespace UtilityLog
{
    public interface IObservableLogger
    {
        IObservable<(LogLevel level, object message)> Messages { get; }
    }
}