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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Text;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Http.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class NameValueCollectionExtensions
    {
        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static string GetParameterValue(this NameValueCollection parameters, string parameterName)
        {
            return GetValue(parameters, parameterName);
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="key">Name of the parameter.</param>
        /// <returns></returns>
        public static string GetValue(this NameValueCollection parameters, string key)
        {
            if (parameters == null)
            {
                return null;
            }

            var value = parameters.Get(key);

            return value.IsNullOrWhiteSpace() ? null : value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestParameters"></param>
        /// <returns></returns>
        public static string ToQueryString(this NameValueCollection requestParameters)
        {
            if (requestParameters == null || requestParameters.Count == 0)
            {
                return null;
            }

            var delimiter = string.Empty;
            
            var stringBuilder = new StringBuilder();

            for (var index = 0; index < requestParameters.Count; ++index)
            {
                stringBuilder
                    .Append(delimiter)
                    .Append(requestParameters.AllKeys[index].ToString(CultureInfo.InvariantCulture))
                    .Append('=')
                    .Append(requestParameters[index].ToString(CultureInfo.InvariantCulture));
                delimiter = "&";
            }
            
            var queryString = stringBuilder.ToString();

            stringBuilder.Length = 0;
            
            return queryString;
        }
        
        internal static NameValueCollection AddPagingParameters(this NameValueCollection parameters, int pageSize, int offset)
        {
            parameters ??= new NameValueCollection();

            if(pageSize <= 0)
            {
                pageSize = RedmineConstants.DEFAULT_PAGE_SIZE_VALUE;
            }
            
            if(offset < 0)
            {
                offset = 0;
            }
            
            parameters.Set(RedmineKeys.LIMIT, pageSize.ToInvariantString());
            parameters.Set(RedmineKeys.OFFSET, offset.ToInvariantString());

            return parameters;
        }

        internal static NameValueCollection AddParamsIfExist(this NameValueCollection parameters, string[] include)
        {
            if (include is not {Length: > 0})
            {
                return parameters;
            }
            
            parameters ??= new NameValueCollection();

            parameters.Add(RedmineKeys.INCLUDE, string.Join(",", include));
            
            return parameters;
        }
        
        internal static void AddIfNotNull(this NameValueCollection nameValueCollection, string key, string value)
        {
            if (!value.IsNullOrWhiteSpace())
            {
                nameValueCollection.Add(key, value);
            }
        }
        
        internal static void AddIfNotNull(this NameValueCollection nameValueCollection, string key, bool? value)
        {
            if (value.HasValue)
            {
                nameValueCollection.Add(key, value.Value.ToInvariantString());
            }
        }   
        
        /// <summary>
        /// Creates a new NameValueCollection with an initial key-value pair.
        /// </summary>
        /// <param name="key">The key for the first item.</param>
        /// <param name="value">The value for the first item.</param>
        /// <returns>A new NameValueCollection containing the specified key-value pair.</returns>
        public static NameValueCollection WithItem(this string key, string value)
        {
            var collection = new NameValueCollection();
            collection.Add(key, value);
            return collection;
        }

        /// <summary>
        /// Adds a new key-value pair to an existing NameValueCollection and returns the collection for chaining.
        /// </summary>
        /// <param name="collection">The NameValueCollection to add to.</param>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>The NameValueCollection with the new key-value pair added.</returns>
        public static NameValueCollection AndItem(this NameValueCollection collection, string key, string value)
        {
            collection.Add(key, value);
            return collection;
        }

        /// <summary>
        /// Adds a new key-value pair to an existing NameValueCollection if the condition is true.
        /// </summary>
        /// <param name="collection">The NameValueCollection to add to.</param>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="key">The key to add if condition is true.</param>
        /// <param name="value">The value to add if condition is true.</param>
        /// <returns>The NameValueCollection, potentially with a new key-value pair added.</returns>
        public static NameValueCollection AndItemIf(this NameValueCollection collection, bool condition, string key, string value)
        {
            if (condition)
            {
                collection.Add(key, value);
            }
            return collection;
        }

        /// <summary>
        /// Adds a new key-value pair to an existing NameValueCollection if the value is not null.
        /// </summary>
        /// <param name="collection">The NameValueCollection to add to.</param>
        /// <param name="key">The key to add if value is not null.</param>
        /// <param name="value">The value to check and add.</param>
        /// <returns>The NameValueCollection, potentially with a new key-value pair added.</returns>
        public static NameValueCollection AndItemIfNotNull(this NameValueCollection collection, string key, string value)
        {
            if (value != null)
            {
                collection.Add(key, value);
            }
            return collection;
        }
        
        /// <summary>
        /// Creates a new NameValueCollection with an initial key-value pair where the value is converted from an integer.
        /// </summary>
        /// <param name="key">The key for the first item.</param>
        /// <param name="value">The integer value to be converted to string.</param>
        /// <returns>A new NameValueCollection containing the specified key-value pair.</returns>
        public static NameValueCollection WithInt(this string key, int value)
        {
            return key.WithItem(value.ToInvariantString());
        }
       
        /// <summary>
        /// Adds a new key-value pair to an existing NameValueCollection where the value is converted from an integer.
        /// </summary>
        /// <param name="collection">The NameValueCollection to add to.</param>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The integer value to be converted to string.</param>
        /// <returns>The NameValueCollection with the new key-value pair added.</returns>
        public static NameValueCollection AndInt(this NameValueCollection collection, string key, int value)
        {
            return collection.AndItem(key, value.ToInvariantString());
        }

        
        /// <summary>
        /// Converts a NameValueCollection to a Dictionary.
        /// </summary>
        /// <param name="collection">The collection to convert.</param>
        /// <returns>A new Dictionary containing the collection's key-value pairs.</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
    
            if (collection != null)
            {
                foreach (string key in collection.Keys)
                {
                    dict[key] = collection[key];
                }
            }
    
            return dict;
        }
        
        /// <summary>
        /// Creates a new NameValueCollection from a dictionary of key-value pairs.
        /// </summary>
        /// <param name="keyValuePairs">Dictionary of key-value pairs to add to the collection.</param>
        /// <returns>A new NameValueCollection containing the specified key-value pairs.</returns>
        public static NameValueCollection ToNameValueCollection(this Dictionary<string, string> keyValuePairs)
        {
            var collection = new NameValueCollection();
    
            if (keyValuePairs != null)
            {
                foreach (var pair in keyValuePairs)
                {
                    collection.Add(pair.Key, pair.Value);
                }
            }
    
            return collection;
        }


    }
}