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

using System;
using System.Collections.Generic;
using Redmine.Net.Api.Logging;
using System.Collections.Specialized;

namespace Redmine.Net.Api.Extensions
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            if (listToClone == null) return null;
            IList<T> clonedList = new List<T>();
            foreach (T item in listToClone)
                clonedList.Add((T)item.Clone());
            return clonedList;
        }

        public static string GetParameterValue(this NameValueCollection parameters, string parameterName)
        {
            if (parameters == null) return null;
            string value = parameters.Get(parameterName);
            return string.IsNullOrEmpty(value) ? null : value;
        }

        public static void UseConsoleLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new ConsoleLogger());
        }

        public static void UseColorConsoleLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new ColorConsoleLogger());
        }

        public static void UseTraceLog(this RedmineManager redmineManager)
        {
            Logger.UseLogger(new TraceLogger());
        }
    }
}