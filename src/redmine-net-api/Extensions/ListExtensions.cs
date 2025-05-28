/*
   Copyright 2011 - 2025 Adrian Popescu

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

using System.Collections.Generic;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// Provides extension methods for operations on lists.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Creates a deep clone of the specified list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list. Must implement <see cref="ICloneable{T}"/>.</typeparam>
        /// <param name="listToClone">The list to be cloned.</param>
        /// <param name="resetId">Specifies whether to reset the ID for each cloned item.</param>
        /// <returns>A new list containing cloned copies of the elements from the original list. Returns null if the original list is null.</returns>
        public static IList<T> Clone<T>(this IList<T> listToClone, bool resetId) where T : ICloneable<T>
        {
            if (listToClone == null)
            {
                return null;
            }

            var clonedList = new List<T>(listToClone.Count);
            
            foreach (var item in listToClone)
            {
                clonedList.Add(item.Clone(resetId));
            }

            return clonedList;
        }

        /// <summary>
        /// Creates a deep clone of the specified list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the list. Must implement <see cref="ICloneable{T}"/>.</typeparam>
        /// <param name="listToClone">The list to be cloned.</param>
        /// <param name="resetId">Specifies whether to reset the ID for each cloned item.</param>
        /// <returns>A new list containing cloned copies of the elements from the original list. Returns null if the original list is null.</returns>
        public static List<T> Clone<T>(this List<T> listToClone, bool resetId) where T : ICloneable<T>
        {
            if (listToClone == null)
            {
                return null;
            }

            var clonedList = new List<T>(listToClone.Count);
            
            foreach (var item in listToClone)
            {
                clonedList.Add(item.Clone(resetId));
            }
            return clonedList;
        }

        /// <summary>
        /// Compares two lists for equality by checking if they contain the same elements in the same order.
        /// </summary>
        /// <typeparam name="T">The type of elements in the lists. Must be a reference type.</typeparam>
        /// <param name="list">The first list to be compared.</param>
        /// <param name="listToCompare">The second list to be compared.</param>
        /// <returns>True if both lists contain the same elements in the same order; otherwise, false. Returns false if either list is null.</returns>
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
        /// Compares two lists for equality based on their elements.
        /// </summary>
        /// <typeparam name="T">The type of elements in the lists. Must be a reference type.</typeparam>
        /// <param name="list">The first list to compare.</param>
        /// <param name="listToCompare">The second list to compare.</param>
        /// <returns>True if both lists are non-null, have the same count, and all corresponding elements are equal; otherwise, false.</returns>
        public static bool Equals<T>(this List<T> list, List<T> listToCompare) where T : class
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
    }
}