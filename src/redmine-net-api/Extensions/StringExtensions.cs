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
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
#if NET20
            if (value == null)
            {
                return true;
            }

            for (var index = 0; index < value.Length; ++index)
            {
                if (!char.IsWhiteSpace(value[index]))
                {
                    return false;
                }
            }
            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string Truncate(this string text, int maximumLength)
        {
            if (!text.IsNullOrWhiteSpace())
            {
                if (text.Length > maximumLength)
                {
                    text = text.Substring(0, maximumLength);
                }
            }

            return text;
        }

        /// <summary>
        /// Lower case based on invariant culture.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "CA1308")]
        public static string ToLowerInv(this string text)
        {
            return text.IsNullOrWhiteSpace() ? text : text.ToLowerInvariant();
        }

        /// <summary>
        /// Transforms a string into a SecureString.
        /// </summary>
        /// <param name = "value">
        /// The string to transform.
        /// </param>
        /// <returns>
        /// A secure string representing the contents of the original string.
        /// </returns>
        internal static SecureString ToSecureString(this string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return null;
            }

            using (var rv = new SecureString())
            {
                foreach (var c in value)
                {
                    rv.AppendChar(c);
                }

                return rv;
            }
        }

        internal static string RemoveTrailingSlash(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            if (s.EndsWith("/", StringComparison.OrdinalIgnoreCase) || s.EndsWith("\"", StringComparison.OrdinalIgnoreCase))
            {
                return s.Substring(0, s.Length - 1);
            }

            return s;
        }
    }
}