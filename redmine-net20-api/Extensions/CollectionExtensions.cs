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

namespace Redmine.Net.Api.Extensions
{
    public static class CollectionExtensions
    {
        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            if (listToClone == null) return null;
            IList<T> clonedList = new List<T>();
            foreach (T item in listToClone)
                clonedList.Add((T)item.Clone());
            return clonedList;
        }

        public static bool Equals<T>(this IList<T> list, IList<T> listToCompare) where T : class
        {
            if (listToCompare == null) return false;

            if (list.Count != listToCompare.Count) return false;

            var index = 0;
            while (index < list.Count &&  (list[index] as T).Equals(listToCompare[index] as T)) index++;
            
            return index == list.Count;
        }
    }
}