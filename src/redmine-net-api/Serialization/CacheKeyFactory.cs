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
using System.Globalization;
using System.Text;
using System.Xml.Serialization;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// The CacheKeyFactory extracts a unique signature
    /// to identify each instance of an XmlSerializer
    /// in the cache.
    /// </summary>
    internal static class CacheKeyFactory
    {

        /// <summary>
        /// Creates a unique signature for the passed
        /// in parameter. MakeKey normalizes array content
        /// and the content of the XmlAttributeOverrides before
        /// creating the key.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="overrides"></param>
        /// <param name="types"></param>
        /// <param name="root"></param>
        /// <param name="defaultNamespace"></param>
        /// <returns></returns>
        public static string Create(Type type, XmlAttributeOverrides overrides, Type[] types, XmlRootAttribute root, string defaultNamespace)
        {
            var keyBuilder = new StringBuilder();
            keyBuilder.Append(type.FullName);
            keyBuilder.Append( "??" );
            keyBuilder.Append(overrides?.GetHashCode().ToString(CultureInfo.InvariantCulture));
            keyBuilder.Append( "??" );
            keyBuilder.Append(GetTypeArraySignature(types));
            keyBuilder.Append("??");
            keyBuilder.Append(root?.GetHashCode().ToString(CultureInfo.InvariantCulture));
            keyBuilder.Append("??");
            keyBuilder.Append(defaultNamespace);

            return keyBuilder.ToString();
        }
        
        /// <summary>
        /// Creates a signature for the passed in Type array. The order
        /// of the elements in the array does not influence the signature.
        /// </summary>
        /// <param name="types"></param>
        /// <returns>An instance independent signature of the Type array</returns>
        public static string GetTypeArraySignature(Type[] types)
        {
            if (null == types || types.Length <= 0)
            {
                return null;
            }

            // to make sure we don't account for the order
            // of the types in the array, we create one SortedList 
            // with the type names, concatenate them and hash that.
            var sorter = new string[types.Length];
            for (var index = 0; index < types.Length; index++)
            {
                Type t = types[index];
                sorter[index] = t.AssemblyQualifiedName;
            }

            Array.Sort(sorter);

            return string.Join(":", sorter);
        }
    }
}