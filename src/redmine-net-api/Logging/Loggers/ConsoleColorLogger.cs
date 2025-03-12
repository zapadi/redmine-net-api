/*
   Copyright 2011 - 2019 Adrian Popescu.

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

namespace Redmine.Net.Api.Logging;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Redmine.Net.Api.Logging.ILogger" />
public sealed class ConsoleColorLogger : ILogger
{
    private static readonly object locker = new object();
    private readonly ConsoleColor? defaultConsoleColor = null;

    /// <summary>
    /// </summary>
    /// <param name="entry"></param>
    public void Log(LogEntry entry)
    {
        lock (locker)
        {
            var colors = GetLogLevelConsoleColors(entry.Severity);
            switch (entry.Severity)
            {
                case LoggingEventType.Debug:
                    Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                    break;
                case LoggingEventType.Information:
                    Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                    break;
                case LoggingEventType.Warning:
                    Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                    break;
                case LoggingEventType.Error:
                    Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                    break;
                case LoggingEventType.Fatal:
                    Console.WriteLine(entry.Message, colors.Background, colors.Foreground);
                    break;
            }
        }
    }

    /// <summary>
    /// Gets the log level console colors.
    /// </summary>
    /// <param name="logLevel">The log level.</param>
    /// <returns></returns>
    private ConsoleColors GetLogLevelConsoleColors(LoggingEventType logLevel)
    {
        return logLevel switch
        {
            LoggingEventType.Fatal => new ConsoleColors(ConsoleColor.White, ConsoleColor.Red),
            LoggingEventType.Error => new ConsoleColors(ConsoleColor.Red, defaultConsoleColor),
            LoggingEventType.Warning => new ConsoleColors(ConsoleColor.DarkYellow, defaultConsoleColor),
            LoggingEventType.Information => new ConsoleColors(ConsoleColor.DarkGreen, defaultConsoleColor),
            _ => new ConsoleColors(ConsoleColor.Gray, defaultConsoleColor)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    private readonly struct ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
    {
        /// <summary>
        /// Gets or sets the foreground.
        /// </summary>
        /// <value>
        /// The foreground.
        /// </value>
        public ConsoleColor? Foreground { get; } = foreground;

        /// <summary>
        /// Gets or sets the background.
        /// </summary>
        /// <value>
        /// The background.
        /// </value>
        public ConsoleColor? Background { get; } = background;
    }
}