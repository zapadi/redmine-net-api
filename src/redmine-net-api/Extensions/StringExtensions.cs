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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security;
using System.Text.RegularExpressions;

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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="maximumLength"></param>
        /// <returns></returns>
        public static string Truncate(this string text, int maximumLength)
        {
            if (text.IsNullOrWhiteSpace() || maximumLength < 1 || text.Length <= maximumLength)
            {
                return text;
            }
            
            #if (NET5_0_OR_GREATER)
            return text.AsSpan()[..maximumLength].ToString();
            #else
            return text.Substring(0, maximumLength);
            #endif
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

            var rv = new SecureString();
            
            for (var index = 0; index < value.Length; ++index)
            {
                rv.AppendChar(value[index]);
            }
            
            return rv;
        }

        internal static string RemoveTrailingSlash(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return s;
            }

            #if (NET5_0_OR_GREATER)
            if (s.EndsWith('/') || s.EndsWith('\\'))
            {
                return s.AsSpan()[..(s.Length - 1)].ToString();
            }
            #else
            if (s.EndsWith("/", StringComparison.OrdinalIgnoreCase) || s.EndsWith(@"\", StringComparison.OrdinalIgnoreCase))
            {
                return s.Substring(0, s.Length - 1);
            }
            #endif
            
            return s;
        }
        
        internal static string ValueOrFallback(this string value, string fallback)
        {
            return !value.IsNullOrWhiteSpace() ? value : fallback;
        }
        
        internal static string ToInvariantString<T>(this T value) where T : struct
        {
            return value switch
            {
                sbyte v => v.ToString(CultureInfo.InvariantCulture),
                byte v => v.ToString(CultureInfo.InvariantCulture),
                short v => v.ToString(CultureInfo.InvariantCulture),
                ushort v => v.ToString(CultureInfo.InvariantCulture),
                int v => v.ToString(CultureInfo.InvariantCulture),
                uint v => v.ToString(CultureInfo.InvariantCulture),
                long v => v.ToString(CultureInfo.InvariantCulture),
                ulong v => v.ToString(CultureInfo.InvariantCulture),
                float v => v.ToString("G7", CultureInfo.InvariantCulture), // Specify precision explicitly for backward compatibility
                double v => v.ToString("G15", CultureInfo.InvariantCulture), // Specify precision explicitly for backward compatibility
                decimal v => v.ToString(CultureInfo.InvariantCulture),
                TimeSpan ts => ts.ToString(),
                DateTime d => d.ToString(CultureInfo.InvariantCulture),
                #pragma warning disable CA1308
                bool b => b.ToString().ToLowerInvariant(),
                #pragma warning restore CA1308
                _ => value.ToString(),
            };
        }

        private const string CRLR = "\r\n";
        private const string CR = "\r";
        private const string LR = "\n";
        
        internal static string ReplaceEndings(this string input, string replacement = CRLR)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return input;
            }

            #if NET6_0_OR_GREATER
            input =  input.ReplaceLineEndings(CRLR);
            #else
            input = Regex.Replace(input, $"{CRLR}|{CR}|{LR}", CRLR);
            #endif
            return input;
        }
    }
}