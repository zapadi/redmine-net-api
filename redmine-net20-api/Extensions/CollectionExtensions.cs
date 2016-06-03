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
        public static bool Equals<T>(this IList<T> leftList, IList<T> rightList) where T : class
        {
            if (rightList == null) return false;

            if (leftList.Count != rightList.Count) return false;

            var index = 0;
            while (index < leftList.Count && leftList[index].Equals(rightList[index])) index++;

            return index == leftList.Count;
        }

        public static IList<T> Clone<T>(this IList<T> list) where T : ICloneable
        {
            if (list == null) return null;
            IList<T> clonedList = new List<T>();
            foreach (var item in list)
                clonedList.Add((T) item.Clone());
            return clonedList;
        }
    }
}