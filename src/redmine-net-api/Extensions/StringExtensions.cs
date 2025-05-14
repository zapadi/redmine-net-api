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
        /// Determines whether a string is null, empty, or consists only of white-space characters.
        /// </summary>
        /// <param name="value">The string to evaluate.</param>
        /// <returns>True if the string is null, empty, or whitespace; otherwise, false.</returns>
        public static bool IsNullOrWhiteSpace(this string value)
        {
            if (value == null)
            {
                return true;
            }

            foreach (var ch in value)
            {
                if (!char.IsWhiteSpace(ch))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Truncates a string to the specified maximum length if it exceeds that length.
        /// </summary>
        /// <param name="text">The string to truncate.</param>
        /// <param name="maximumLength">The maximum allowed length for the string.</param>
        /// <returns>The truncated string if its length exceeds the maximum length; otherwise, the original string.</returns>
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
            
            foreach (var ch in value)
            {
                rv.AppendChar(ch);
            }
            
            return rv;
        }

        /// <summary>
        /// Removes the trailing slash ('/' or '\') from the end of the string if it exists.
        /// </summary>
        /// <param name="s">The string to process.</param>
        /// <returns>The input string without a trailing slash, or the original string if no trailing slash exists.</returns>
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

        /// <summary>
        /// Returns the specified string value if it is neither null, empty, nor consists only of white-space characters; otherwise, returns the fallback string.
        /// </summary>
        /// <param name="value">The primary string value to evaluate.</param>
        /// <param name="fallback">The fallback string to return if the primary string is null, empty, or consists of only white-space characters.</param>
        /// <returns>The original string if it is valid; otherwise, the fallback string.</returns>
        internal static string ValueOrFallback(this string value, string fallback)
        {
            return !value.IsNullOrWhiteSpace() ? value : fallback;
        }

        /// <summary>
        /// Converts a value of a struct type to its invariant culture string representation.
        /// </summary>
        /// <typeparam name="T">The struct type of the value.</typeparam>
        /// <param name="value">The value to convert to a string.</param>
        /// <returns>The invariant culture string representation of the value.</returns>
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
                bool b => b ? "true" : "false",
                #pragma warning restore CA1308
                _ => value.ToString(),
            };
        }

        private const string CR = "\r";
        private const string LR = "\n";
        private const string CRLR = $"{CR}{LR}";

        /// <summary>
        /// Replaces all line endings in the input string with the specified replacement string.
        /// </summary>
        /// <param name="input">The string in which line endings will be replaced.</param>
        /// <param name="replacement">The string to replace line endings with. Defaults to a combination of carriage return and line feed.</param>
        /// <returns>The input string with all line endings replaced by the specified replacement string.</returns>
        internal static string ReplaceEndings(this string input, string replacement = CRLR)
        {
            if (input.IsNullOrWhiteSpace())
            {
                return input;
            }

            #if NET6_0_OR_GREATER
            input =  input.ReplaceLineEndings(replacement);
            #else
            input = Regex.Replace(input, $"{CRLR}|{CR}|{LR}", replacement);
            #endif
            return input;
        }
    }
}