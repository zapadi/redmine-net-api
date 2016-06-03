using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Redmine.Net.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static bool Equals<T>(this IList<T> list, IList<T> listToCompare) where T : class
        {
            if (listToCompare == null) return false;

            var set = new HashSet<T>(list);
            var setToCompare = new HashSet<T>(listToCompare);

            return set.SetEquals(setToCompare);
        }
    }
}
