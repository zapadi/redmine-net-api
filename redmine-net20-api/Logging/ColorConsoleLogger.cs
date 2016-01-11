using System;

namespace Redmine.Net.Api.Logging
{
    public class ColorConsoleLogger : ILogger
    {
        static readonly object locker = new object();
        private readonly ConsoleColor? defaultConsoleColor = null;

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

        private ConsoleColors GetLogLevelConsoleColors(LoggingEventType logLevel)
        {
            // do not change user's background color except for Critical
            switch (logLevel)
            {
                case LoggingEventType.Fatal:
                    return new ConsoleColors(ConsoleColor.White, ConsoleColor.Red);
                case LoggingEventType.Error:
                    return new ConsoleColors(ConsoleColor.Red, defaultConsoleColor);
                case LoggingEventType.Warning:
                    return new ConsoleColors(ConsoleColor.DarkYellow, defaultConsoleColor);
                case LoggingEventType.Information:
                    return new ConsoleColors(ConsoleColor.DarkGreen, defaultConsoleColor);
                default:
                    return new ConsoleColors(ConsoleColor.Gray, defaultConsoleColor);
            }
        }

        private struct ConsoleColors
        {
            public ConsoleColors(ConsoleColor? foreground, ConsoleColor? background)
            {
                Foreground = foreground;
                Background = background;
            }

            public ConsoleColor? Foreground { get; }

            public ConsoleColor? Background { get; }
        }
    }
}