/*
   Copyright 2011 - 2016 Adrian Popescu.

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/

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