/*
   Copyright 2011 - 2022 Adrian Popescu

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

namespace Redmine.Net.Api.Internals
{
    /// <summary>
    /// 
    /// </summary>
    internal static class HashCodeHelper
    {
        /// <summary>
        /// Returns a hash code for the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public static int GetHashCode<T>(IList<T> list, int hash) where T : class
        {
            unchecked
            {
                var hashCode = hash;
                if (list == null) return hashCode;
                hashCode = (hashCode * 13) + list.Count;
                foreach (var t in list)
                {
                    hashCode *= 13;
                    if (t != null) hashCode += t.GetHashCode();
                }

                return hashCode;
            }
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <param name="hash">The hash.</param>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public static int GetHashCode<T>(T entity, int hash)
        {
            unchecked
            {
                var hashCode = hash;

                var type = typeof(T);

                var isNullable = Nullable.GetUnderlyingType(type) != null;
                if (isNullable)
                {
                    type = type.UnderlyingSystemType;
                }

                if (type.IsValueType)
                {
                    hashCode = (hashCode * 397) ^ entity.GetHashCode();
                }
                else
                {
                    hashCode = (hashCode * 397) ^ (entity?.GetHashCode() ?? 0);
                }

                return hashCode;
            }
        }
    }
}