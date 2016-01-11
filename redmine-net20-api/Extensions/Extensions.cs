using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Redmine.Net.Api.Logging;

namespace Redmine.Net.Api.Extensions
{
    public static class Extensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
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
            Logging.Logger.UseLogger(new ConsoleLogger());
        }

        public static void UseColorConsoleLog(this RedmineManager redmineManager)
        {
            Logging.Logger.UseLogger(new ColorConsoleLogger());
        }

        public static void UseTraceLog(this RedmineManager redmineManager)
        {
            Logging.Logger.UseLogger(new TraceLogger());
        }
    }
}