using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using Splat;

namespace UtilityLog
{
    public class ObservableLogger : ILogger, IObservableLogger
    {
        //public static readonly ObservableLogger Instance = new ObservableLogger();
        private readonly ISubject<(LogLevel level, object message), (LogLevel level, object message)> messages;

        protected ObservableLogger()
        {
            this.messages = Subject.Synchronize(new ReplaySubject<(LogLevel level, object message)>());
        }

        public IObservable<(LogLevel level, object message)> Messages => this.messages;

        public LogLevel Level
        {
            get;
            set;
        }

        public void Write(string message, LogLevel logLevel)
        {
            if (logLevel < this.Level) return;

            this.messages.OnNext((logLevel, message));
        }

        public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel)
        {
            if (logLevel < this.Level) return;

            this.messages.OnNext((logLevel, new Exception(message, exception)));
        }

        public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            if (logLevel < this.Level) return;

            this.messages.OnNext((logLevel, message));
        }

        public void Write(System.Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            if (logLevel < this.Level) return;

            this.messages.OnNext((logLevel, new Exception(message, exception)));
        }
    }
}