using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using Splat;

namespace Utility.Log.Logger {
   public class ObservableLogger : ILogger, IObservableLogger {
      private readonly ISubject<(LogLevel level, object message, DateTime date)> messages;

      protected ObservableLogger() {
         this.messages = Subject.Synchronize(new ReplaySubject<(LogLevel level, object message, DateTime date)>());
      }

      public IObservable<(LogLevel level, object message, DateTime date)> Messages => this.messages;

      public LogLevel Level {
         get;
         set;
      }

      public void Write(string message, LogLevel logLevel) {
         if (logLevel < this.Level) return;

         this.messages.OnNext((logLevel, message, DateTime.Now));
      }

      public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel) {
         if (logLevel < this.Level) return;

         this.messages.OnNext((logLevel, new Exception(message, exception), DateTime.Now));
      }

      public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel) {
         if (logLevel < this.Level) return;

         this.messages.OnNext((logLevel, message, DateTime.Now));
      }

      public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel) {
         if (logLevel < this.Level) return;

         this.messages.OnNext((logLevel, new Exception(message, exception), DateTime.Now));
      }
   }
}