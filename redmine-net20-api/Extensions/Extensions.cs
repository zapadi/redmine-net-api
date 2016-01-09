using System;
using System.Collections.Generic;
using System.Collections.Specialized;

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
    }
}