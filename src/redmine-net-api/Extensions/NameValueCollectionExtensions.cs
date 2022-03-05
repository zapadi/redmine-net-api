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
            if (parameters == null)
            {
                return null;
            }

            var value = parameters.Get(parameterName);
            
            return value.IsNullOrWhiteSpace() ? null : value;
        }

        /// <summary>
        /// Gets the parameter value.
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <returns></returns>
        public static string GetValue(this NameValueCollection parameters, string parameterName)
        {
            if (parameters == null)
            {
                return null;
            }

            var value = parameters.Get(parameterName);

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

            var stringBuilder = new StringBuilder();

            for (var index = 0; index < requestParameters.Count; ++index)
            {
                stringBuilder
                    .Append(requestParameters.AllKeys[index].ToString(CultureInfo.InvariantCulture))
                    .Append("=")
                    .Append(requestParameters[index].ToString(CultureInfo.InvariantCulture))
                    .Append("&");
            }

            stringBuilder.Length -= 1;

            var queryString = stringBuilder.ToString();

            #if !(NET20)
            stringBuilder.Clear();
            #endif
            stringBuilder = null;
            
            return queryString;
        }

    }
}