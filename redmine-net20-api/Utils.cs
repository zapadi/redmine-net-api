using System;
using System.Collections.Generic;

namespace Redmine.Net.Api
{
    public static class Utils
    {
        public static int GetHashCode<T>(this IList<T> list, int hash)
        {
            unchecked
            {
                var hashCode = hash;
                if (list != null)
                {
                    hashCode = (hashCode * 13) + list.Count;
                    foreach (T t in list)
                    {
                        hashCode *= 13;
                        if (t != null) hashCode = hashCode + t.GetHashCode();
                    }
                }

                return hashCode;
            }
        }

		public static int GetHashCode<T>(this T entity, int hash) 
        {
            unchecked
            {
                var hashCode = hash;

				hashCode = (hashCode * 397) ^ (entity.Equals( default(T)) ? 0 : entity.GetHashCode());

                return hashCode;
            }
        }
    }
}