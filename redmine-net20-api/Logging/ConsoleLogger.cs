using System;

namespace Redmine.Net.Api.Logging
{
    public class ConsoleLogger : ILogger
    {
        static readonly object locker = new object();
        public void Log(LogEntry entry)
        {
            lock (locker)
            {
                switch (entry.Severity)
                {
                    case LoggingEventType.Debug:
                        Console.WriteLine(entry.Message);
                        break;
                    case LoggingEventType.Information:
                        Console.WriteLine(entry.Message);
                        break;
                    case LoggingEventType.Warning:
                        Console.WriteLine(entry.Message);
                        break;
                    case LoggingEventType.Error:
                        Console.WriteLine(entry.Message);
                        break;
                    case LoggingEventType.Fatal:
                        Console.WriteLine(entry.Message);
                        break;
                }
            }
        }
    }
}