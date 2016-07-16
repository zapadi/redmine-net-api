/*
   Copyright 2011 - 2016 Adrian Popescu

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

using Redmine.Net.Api.Logging;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// </summary>
    public static class LoggerExtensions
    {
        /// <summary>
        ///     Uses the console log.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        public static void UseConsoleLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new ConsoleLogger());
        }

        /// <summary>
        ///     Uses the color console log.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        public static void UseColorConsoleLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new ColorConsoleLogger());
        }

        /// <summary>
        ///     Uses the trace log.
        /// </summary>
        /// <param name="redmineManager">The redmine manager.</param>
        public static void UseTraceLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new TraceLogger());
        }
    }
}