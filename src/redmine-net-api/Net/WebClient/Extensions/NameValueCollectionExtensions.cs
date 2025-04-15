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

using System.Collections.Specialized;
using System.Globalization;
using System.Text;

namespace Redmine.Net.Api.Extensions
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
            
            parameters.Set(RedmineKeys.LIMIT, pageSize.ToString(CultureInfo.InvariantCulture));
            parameters.Set(RedmineKeys.OFFSET, offset.ToString(CultureInfo.InvariantCulture));

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
                nameValueCollection.Add(key, value.Value.ToString(CultureInfo.InvariantCulture));
            }
        }   
    }
}