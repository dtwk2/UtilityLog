using System;
using Splat;

namespace Utility.Log.Logger
{
    public interface IObservableLogger
    {
        IObservable<(LogLevel level, object message, DateTime date)> Messages { get; }
    }
}