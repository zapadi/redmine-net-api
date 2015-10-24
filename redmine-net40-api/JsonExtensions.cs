using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using Redmine.Net.Api.JSonConverters;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api
{
    public static class JsonExtensions
    {
        public static void WriteIdIfNotNull(this Dictionary<string, object> dictionary, IdentifiableName ident, String key)
        {
            if (ident != null) dictionary.Add(key, ident.Id);
        }

        public static void WriteIfNotDefaultOrNull<T>(this Dictionary<string, object> dictionary, T? val, String tag) where T : struct
        {
            if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default(T)))
                dictionary.Add(tag, string.Empty);
            else
                dictionary.Add(tag, val.Value);
        }

        public static void WriteIdOrEmpty(this Dictionary<string, object> dictionary, IdentifiableName ident, String key, String emptyValue = null)
        {
            if (ident != null) dictionary.Add(key, ident.Id);
            else dictionary.Add(key, emptyValue);
        }

        public static void WriteArray<T>(this Dictionary<string, object> dictionary, string key, IEnumerable<T> col,
            JavaScriptConverter converter, JavaScriptSerializer serializer)
        {
            if (col != null)
            {
                serializer.RegisterConverters(new[] { converter });
                dictionary.Add(key, col.ToArray());
            }
        }

        public static void WriteIdsArray(this Dictionary<string, object> dictionary, string key,
            IEnumerable<IdentifiableName> coll)
        {
            if (coll != null)
                dictionary.Add(key, coll.Select(x => x.Id).ToArray());
        }

        public static void WriteNamesArray(this Dictionary<string, object> dictionary, string key,
            IEnumerable<IdentifiableName> coll)
        {
            if (coll != null)
                dictionary.Add(key, coll.Select(x => x.Name).ToArray());
        }

        public static void WriteDateOrEmpty(this Dictionary<string, object> dictionary, DateTime? val, String tag)
        {
            if (!val.HasValue || val.Value.Equals(default(DateTime)))
                dictionary.Add(tag, string.Empty);
            else
                dictionary.Add(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }

        public static void WriteValueOrEmpty<T>(this Dictionary<string, object> dictionary, T? val, String tag) where T : struct
        {
            if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default(T)))
                dictionary.Add(tag, string.Empty);
            else
                dictionary.Add(tag, val.Value);
        }

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

            return (T)Convert.ChangeType(val, type);
        }

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
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static List<T> GetValueAsCollection<T>(this IDictionary<string, object> dictionary, string key) where T : new()
        {
            object val;

            if (!dictionary.TryGetValue(key, out val)) return null;

            var ser = new JavaScriptSerializer();
            ser.RegisterConverters(new[] { RedmineSerialization.Converters[typeof(T)] });

            List<T> list = new List<T>();

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