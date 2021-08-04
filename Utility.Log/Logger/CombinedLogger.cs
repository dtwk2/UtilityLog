using System;
using System.ComponentModel;
using Splat;

namespace Utility.Log.Logger
{
    public class CombinedLogger : ILogger
    {
        private readonly ILogger[] loggers;

        public CombinedLogger(params ILogger[] loggers)
        {
            this.loggers = loggers;
        }

        public LogLevel Level => LogLevel.Debug;

        public void Write([Localizable(false)] string message, LogLevel logLevel)
        {
            try
            {
                foreach (var logger in loggers)
                    logger.Write(message, logLevel);
            }
            catch (Exception ex)
            {
            }
        }

        public void Write(Exception exception, [Localizable(false)] string message, LogLevel logLevel)
        {
            try
            {
                foreach (var logger in loggers)
                    logger.Write(exception, message, logLevel);
            }
            catch (Exception ex)
            {
            }
        }

        public void Write([Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            try
            {
                foreach (var logger in loggers)
                    logger.Write(message, type, logLevel);
            }
            catch (Exception ex)
            {
            }
        }

        public void Write(Exception exception, [Localizable(false)] string message, [Localizable(false)] Type type, LogLevel logLevel)
        {
            try
            {
                foreach (var logger in loggers)
                    logger.Write(exception, message, type, logLevel);
            }
            catch (Exception ex)
            {
            }
        }
    }
}