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
    /// <summary>
    /// 
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        public static void Debug(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message));
        }

        /// <summary>
        /// Debugs the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Debug(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, message, exception));
        }

        /// <summary>
        /// Debugs the specified format provider.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Debug(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, string.Format(formatProvider, format, args)));
        }

        /// <summary>
        /// Debugs the specified format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Debug(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Debug, string.Format(CultureInfo.CurrentCulture, format, args)));
        }

        /// <summary>
        /// Informations the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        public static void Information(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, message));
        }

        /// <summary>
        /// Informations the specified format provider.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Information(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, string.Format(formatProvider, format, args)));
        }

        /// <summary>
        /// Informations the specified format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Information(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Information, string.Format(CultureInfo.CurrentCulture, format, args)));
        }

        /// <summary>
        /// Warnings the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        public static void Warning(this ILogger logger, string message)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, message));
        }

        /// <summary>
        /// Warnings the specified format provider.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warning(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, string.Format(formatProvider, format, args)));
        }

        /// <summary>
        /// Warnings the specified format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Warning(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Warning, string.Format(CultureInfo.CurrentCulture, format, args)));
        }

        /// <summary>
        /// Errors the specified exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        public static void Error(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, exception.Message, exception));
        }

        /// <summary>
        /// Errors the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Error(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, message, exception));
        }

        /// <summary>
        /// Errors the specified format provider.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, string.Format(formatProvider, format, args)));
        }

        /// <summary>
        /// Errors the specified format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Error(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Error, string.Format(CultureInfo.CurrentCulture, format, args)));
        }

        /// <summary>
        /// Fatals the specified exception.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="exception">The exception.</param>
        public static void Fatal(this ILogger logger, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, exception.Message, exception));
        }

        /// <summary>
        /// Fatals the specified message.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="message">The message.</param>
        /// <param name="exception">The exception.</param>
        public static void Fatal(this ILogger logger, string message, Exception exception)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, message, exception));
        }

        /// <summary>
        /// Fatals the specified format provider.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Fatal(this ILogger logger, IFormatProvider formatProvider, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, string.Format(formatProvider, format, args)));
        }

        /// <summary>
        /// Fatals the specified format.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="format">The format.</param>
        /// <param name="args">The arguments.</param>
        public static void Fatal(this ILogger logger, string format, params object[] args)
        {
            logger.Log(new LogEntry(LoggingEventType.Fatal, string.Format(CultureInfo.CurrentCulture, format, args)));
        }
    }
}