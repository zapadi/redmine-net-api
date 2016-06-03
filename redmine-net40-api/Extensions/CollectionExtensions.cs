using System;
using System.Collections.Generic;
using System.Linq;

namespace Redmine.Net.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static bool Equals<T>(this IList<T> leftCollection, IList<T> rightCollection) where T : class
        {
            if (rightCollection == null) return false;

            var set = new HashSet<T>(leftCollection);
            var setToCompare = new HashSet<T>(rightCollection);

            return set.SetEquals(setToCompare);
        }

        public static IList<T> Clone<T>(this IList<T> collection) where T : ICloneable
        {
            return collection == null ? null : collection.Select(item => (T) item.Clone()).ToList();
        }
    }
}