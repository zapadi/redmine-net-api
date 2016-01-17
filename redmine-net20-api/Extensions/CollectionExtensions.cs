using System;
using System.Collections.Generic;
using System.Text;

namespace Redmine.Net.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static bool Equals<T>(this IList<T> list, IList<T> listToCompare) where T : class
        {
            if (listToCompare == null) return false;

            if (list.Count != listToCompare.Count) return false;

            int index = 0;
            while (index < list.Count &&  (list[index] as T).Equals(listToCompare[index] as T)) index++;
            
            if (index != list.Count) return false;

            return true;


           
        }
    }
}
