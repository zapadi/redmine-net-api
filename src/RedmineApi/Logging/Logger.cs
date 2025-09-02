﻿/*
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

namespace Redmine.Net.Api.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public static class Logger
    {
        private static readonly object locker = new object();
        private static ILogger logger;

        /// <summary>
        /// Gets the current ILogger.
        /// </summary>
        /// <value>
        /// The current.
        /// </value>
        public static ILogger Current
        {
            get { return logger ?? (logger = new ConsoleLogger()); }
            private set { logger = value; }
        }

        /// <summary>
        /// Uses the logger.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public static void UseLogger(ILogger logger)
        {
            lock (locker)
            {
                Current = logger;
            }
        }
    }
}