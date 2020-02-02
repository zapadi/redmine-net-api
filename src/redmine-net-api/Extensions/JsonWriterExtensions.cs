using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Redmine.Net.Api.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class JsonExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public static void WriteIdIfNotNull(this JsonWriter jsonWriter, string tag, IdentifiableName value)
        {
            if (value != null)
            {
                jsonWriter.WritePropertyName(tag);
                jsonWriter.WriteValue(value.Id);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        public static void WriteIfNotDefaultOrNull<T>(this JsonWriter writer, string elementName, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default(T)))
            {
                return;
            }

            if (value is bool)
            {
                writer.WriteProperty(elementName, value.ToString().ToLowerInv());
                return;
            }

            writer.WriteProperty(elementName, string.Format(CultureInfo.InvariantCulture, "{0}", value.ToString()));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="ident"></param>
        /// <param name="emptyValue"></param>
        public static void WriteIdOrEmpty(this JsonWriter jsonWriter, string tag, IdentifiableName ident, string emptyValue = null)
        {
            if (ident != null)
            {
                jsonWriter.WriteProperty(tag, ident.Id.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                jsonWriter.WriteProperty(tag, emptyValue);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        /// <param name="dateFormat"></param>
        public static void WriteDateOrEmpty(this JsonWriter jsonWriter, string tag, DateTime? val, string dateFormat = "yyyy-MM-dd")
        {
            if (!val.HasValue || val.Value.Equals(default(DateTime)))
            {
                jsonWriter.WriteProperty(tag, string.Empty);
            }
            else
            {
                jsonWriter.WriteProperty(tag,  val.Value.ToString(dateFormat, CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public static void WriteValueOrEmpty<T>(this JsonWriter jsonWriter, string tag, T? val) where T : struct
        {
            if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default(T)))
            {
                jsonWriter.WriteProperty(tag, string.Empty);
            }
            else
            {
                jsonWriter.WriteProperty(tag, val.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="val"></param>
        public static void WriteValueOrDefault<T>(this JsonWriter jsonWriter, string tag, T? val) where T : struct
        {
            jsonWriter.WriteProperty(tag, val ?? default(T));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        public static void WriteProperty(this JsonWriter jsonWriter, string tag, object value)
        {
            jsonWriter.WritePropertyName(tag);
            jsonWriter.WriteValue(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        public static void WriteRepeatableElement(this JsonWriter jsonWriter, string tag, IEnumerable<IValue> collection)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var value in collection)
            {
                jsonWriter.WritePropertyName(tag);
                jsonWriter.WriteValue(value.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        public static void WriteArrayIds(this JsonWriter jsonWriter, string tag, IEnumerable<IdentifiableName> collection)
        {
            if (collection == null)
            {
                return;
            }

            jsonWriter.WritePropertyName(tag);
            jsonWriter.WriteStartArray();

            StringBuilder sb = new StringBuilder();

            foreach (var identifiableName in collection)
            {
                sb.Append(identifiableName.Id.ToString(CultureInfo.InvariantCulture)).Append(",");
            }

            sb.Length -= 1;
            jsonWriter.WriteValue(sb.ToString());
            sb= null;

            jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        public static void WriteArrayNames(this JsonWriter jsonWriter, string tag, IEnumerable<IdentifiableName> collection)
        {
            if (collection == null)
            {
                return;
            }

            jsonWriter.WritePropertyName(tag);
            jsonWriter.WriteStartArray();

            StringBuilder sb = new StringBuilder();

            foreach (var identifiableName in collection)
            {
                sb.Append(identifiableName.Name).Append(",");
            }

            sb.Length -= 1;
            jsonWriter.WriteValue(sb.ToString());
            sb = null;

            jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        public static void WriteArray<T>(this JsonWriter jsonWriter, string tag, ICollection<T> collection) where T : IJsonSerializable
        {
            if (collection == null)
            {
                return;
            }

            jsonWriter.WritePropertyName(tag);
            jsonWriter.WriteStartArray();

            foreach (var item in collection)
            {
                item.WriteJson(jsonWriter);
            }

            jsonWriter.WriteEndArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonWriter"></param>
        /// <param name="tag"></param>
        /// <param name="collection"></param>
        public static void WriteListAsProperty(this JsonWriter jsonWriter, string tag, ICollection collection)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                jsonWriter.WriteProperty(tag, item);
            }
        }
    }
}