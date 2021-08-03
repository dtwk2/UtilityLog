using Splat;
using System;

namespace Pcs.Hfrr.Log
{
    public interface IObservableLogger
    {
        IObservable<(LogLevel level, object message, DateTime date)> Messages { get; }
    }
}