using System;
using System.Globalization;

namespace Redmine.Net.Api.Logging
{
    public static class LoggerExtensions
    {
        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message));
        }

        public static void Debug(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message, exception));
        }

        public static void Debug(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, string.Format(formatProvider, format, args)));
        }

        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, string.Format(CultureInfo.CurrentCulture, format, args)));
        }


        public static void Information(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message));
        }

        public static void Information(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, string.Format(formatProvider, format, args)));
        }

        public static void Information(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, string.Format(CultureInfo.CurrentCulture, format, args)));
        }


        public static void Warning(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message));
        }

        public static void Warning(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, string.Format(formatProvider, format, args)));
        }

        public static void Warning(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, string.Format(CultureInfo.CurrentCulture, format, args)));
        }


        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, exception));
        }

        public static void Error(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, message, exception));
        }

        public static void Error(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, string.Format(formatProvider, format, args)));
        }

        public static void Error(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, string.Format(CultureInfo.CurrentCulture, format, args)));
        }


        public static void Fatal(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, exception.Message, exception));
        }

        public static void Fatal(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, message, exception));
        }

        public static void Fatal(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, string.Format(formatProvider, format, args)));
        }

        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, string.Format(CultureInfo.CurrentCulture, format, args)));
        }
    }
}