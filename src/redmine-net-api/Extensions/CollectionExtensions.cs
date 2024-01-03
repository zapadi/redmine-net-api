/*
   Copyright 2011 - 2023 Adrian Popescu

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
using System.Text;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        ///     Clones the specified list to clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="listToClone">The list to clone.</param>
        /// <returns></returns>
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            if (listToClone == null)
            {
                return null;
            }
            
            var clonedList = new List<T>();
            
            for (var index = 0; index < listToClone.Count; index++)
            {
                var item = listToClone[index];
                clonedList.Add((T) item.Clone());
            }

            return clonedList;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="listToCompare">The list to compare.</param>
        /// <returns></returns>
        public static bool Equals<T>(this IList<T> list, IList<T> listToCompare) where T : class
        {
            if (list == null || listToCompare == null)
            {
                return false;
            }
            
            if (list.Count != listToCompare.Count)
            {
                return false;
            }
            
            var index = 0;
            while (index < list.Count && list[index].Equals(listToCompare[index]))
            {
                index++;
            }

            return index == list.Count;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        public static string Dump<TIn>(this IEnumerable<TIn> collection) where TIn : class
        {
            if (collection == null)
            {
                return null;
            }

            var sb = new StringBuilder("{");
            
            foreach (var item in collection)
            {
                sb.Append(item).Append(',');
            }

            if (sb.Length > 1)
            {
                sb.Length -= 1;
            }
            
            sb.Append('}');

            var str = sb.ToString();
            sb.Length = 0;

            return str;
        }
    }
}