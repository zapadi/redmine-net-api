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

namespace Redmine.Net.Api.Logging
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Redmine.Net.Api.Logging.ILogger" />
    public sealed class ConsoleLogger : ILogger
    {
        private static readonly object locker = new object();
        /// <summary>
        /// </summary>
        /// <param name="entry"></param>
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