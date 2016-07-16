/*
   Copyright 2011 - 2016 Adrian Popescu.

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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.JSonConverters;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class JsonExtensions
    {
        /// <summary>
        /// Writes the identifier if not null.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="ident">The ident.</param>
        /// <param name="key">The key.</param>
        public static void WriteIdIfNotNull(this Dictionary<string, object> dictionary, IdentifiableName ident, string key)
        {
            if (ident != null) dictionary.Add(key, ident.Id);
        }

        /// <summary>
        /// Writes the identifier or empty.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="ident">The ident.</param>
        /// <param name="key">The key.</param>
        /// <param name="emptyValue">The empty value.</param>
        public static void WriteIdOrEmpty(this Dictionary<string, object> dictionary, IdentifiableName ident, string key, string emptyValue = null)
        {
            if (ident != null) dictionary.Add(key, ident.Id);
            else dictionary.Add(key, emptyValue);
        }

        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="col">The col.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="serializer">The serializer.</param>
        public static void WriteArray<T>(this Dictionary<string, object> dictionary, string key, IEnumerable<T> col,
            JavaScriptConverter converter, JavaScriptSerializer serializer)
        {
            if (col != null)
            {
                serializer.RegisterConverters(new[] { converter });
                dictionary.Add(key, col.ToArray());
            }
        }

        /// <summary>
        /// Writes the ids array.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="coll">The coll.</param>
        public static void WriteIdsArray(this Dictionary<string, object> dictionary, string key,
            IEnumerable<IdentifiableName> coll)
        {
            if (coll != null)
                dictionary.Add(key, coll.Select(x => x.Id).ToArray());
        }

        /// <summary>
        /// Writes the names array.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="coll">The coll.</param>
        public static void WriteNamesArray(this Dictionary<string, object> dictionary, string key,
            IEnumerable<IdentifiableName> coll)
        {
            if (coll != null)
                dictionary.Add(key, coll.Select(x => x.Name).ToArray());
        }

        /// <summary>
        /// Writes the date or empty.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteDateOrEmpty(this Dictionary<string, object> dictionary, DateTime? val, string tag)
        {
            if (!val.HasValue || val.Value.Equals(default(DateTime)))
                dictionary.Add(tag, string.Empty);
            else
                dictionary.Add(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// Writes the value or empty.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteValueOrEmpty<T>(this Dictionary<string, object> dictionary, T? val, string tag) where T : struct
        {
            if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default(T)))
                dictionary.Add(tag, string.Empty);
            else
                dictionary.Add(tag, val.Value);
        }

        /// <summary>
        /// Writes the value or default.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteValueOrDefault<T>(this Dictionary<string, object> dictionary, T? val, string tag) where T : struct
        {
            dictionary.Add(tag, val ?? default(T));
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static T GetValue<T>(this IDictionary<string, object> dictionary, string key)
        {
            object val;
            var dict = dictionary;
            var type = typeof(T);
            if (!dict.TryGetValue(key, out val)) return default(T);

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (val == null) return default(T);

                type = Nullable.GetUnderlyingType(type);
            }

            if (val.GetType() == typeof(ArrayList)) return (T)val;

            if (type.IsEnum) val = Enum.Parse(type, val.ToString(), true);

            return (T)Convert.ChangeType(val, type, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the name of the value as identifiable.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static IdentifiableName GetValueAsIdentifiableName(this IDictionary<string, object> dictionary, string key)
        {
            object val;
            if (!dictionary.TryGetValue(key, out val)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { new IdentifiableNameConverter() });

            var result = ser.ConvertToType<IdentifiableName>(val);
            return result;
        }

        /// <summary>
        /// For Json
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static List<T> GetValueAsCollection<T>(this IDictionary<string, object> dictionary, string key) where T : new()
        {
            object val;
            if (!dictionary.TryGetValue(key, out val)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { RedmineSerializer.JsonConverters[typeof(T)] });

            var list = new List<T>();

            var arrayList = val as ArrayList;
            if (arrayList != null)
            {
                list.AddRange(from object item in arrayList select ser.ConvertToType<T>(item));
            }
            else
            {
                var dict = val as Dictionary<string, object>;
                if (dict != null)
                {
                    list.AddRange(dict.Select(pair => ser.ConvertToType<T>(pair.Value)));
                }
            }
            return list;
        }
    }
}