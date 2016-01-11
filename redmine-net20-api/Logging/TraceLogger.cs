using System;
using System.Diagnostics;

namespace Redmine.Net.Api.Logging
{
    public class TraceLogger : ILogger
    {
        public void Log(LogEntry entry)
        {
            switch (entry.Severity)
            {
                case LoggingEventType.Debug:
                    Trace.WriteLine(entry.Message, "Debug");
                    break;
                case LoggingEventType.Information:
                    Trace.TraceInformation(entry.Message);
                    break;
                case LoggingEventType.Warning:
                    Trace.TraceWarning(entry.Message);
                    break;
                case LoggingEventType.Error:
                    Trace.TraceError(entry.Message);
                    break;
                case LoggingEventType.Fatal:
                    Trace.WriteLine(entry.Message, "Fatal");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}