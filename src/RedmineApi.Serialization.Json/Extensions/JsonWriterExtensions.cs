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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.Json.Extensions;

/// <summary>
/// 
/// </summary>
internal static class JsonWriterExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteIdIfNotNull(this JsonWriter jsonWriter, string tag, IdentifiableName? value)
    {
        if (value == null)
        {
            return;
        }

        jsonWriter.WritePropertyName(tag);
        jsonWriter.WriteValue(value.Id);
    }

    /// <summary>
    /// Writes if not default or null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The property name.</param>
    public static void WriteIfNotDefaultOrNull<T>(this JsonWriter writer, string elementName, T value)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            return;
        }

        if (value is bool boolValue)
        {
            writer.WriteProperty(elementName, boolValue.ToInvariantString());
        }
        else
        {
            writer.WriteProperty(elementName, value.ToString());
        }
    }

    /// <summary>
    /// Writes the boolean value
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The property name.</param>
    public static void WriteBoolean(this JsonWriter writer, string elementName, bool value)
    {
        writer.WriteProperty(elementName, value.ToInvariantString());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="ident"></param>
    /// <param name="emptyValue"></param>
    public static void WriteIdOrEmpty(this JsonWriter jsonWriter, string tag, IdentifiableName? ident, string? emptyValue = null)
    {
        jsonWriter.WriteProperty(tag, ident != null ? ident.Id.ToInvariantString() : emptyValue);
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
        if (!val.HasValue || val.Value.Equals(default))
        {
            jsonWriter.WriteProperty(tag, string.Empty);
        }
        else
        {
            jsonWriter.WriteProperty(tag, val.Value.ToString(dateFormat, CultureInfo.InvariantCulture));
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
        if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default))
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
        jsonWriter.WritePropertyName(tag);
        if (val.HasValue)
        {
            jsonWriter.WriteValue(val);
        }
        else
        {
            var def = default(T);
            jsonWriter.WriteValue(def);
        }
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
    /// <param name="value"></param>
    public static void WriteProperty(this JsonWriter jsonWriter, string tag, int value)
    {
        jsonWriter.WritePropertyName(tag);
        jsonWriter.WriteValue(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteProperty(this JsonWriter jsonWriter, string tag, bool value)
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
    public static void WriteRepeatableElement(this JsonWriter jsonWriter, string tag, IEnumerable<IValue>? collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (var value in collection)
        {
            jsonWriter.WriteProperty(tag, value.Value);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteArrayIds(this JsonWriter jsonWriter, string tag, IEnumerable<IdentifiableName>? collection)
    {
        if (collection == null)
        {
            return;
        }

        jsonWriter.WritePropertyName(tag);
        jsonWriter.WriteStartArray();

        var sb = new StringBuilder();

        foreach (var identifiableName in collection)
        {
            sb.Append(identifiableName.Id.ToInvariantString()).Append(',');
        }

        if (sb.Length > 1)
        {
            sb.Length -= 1;
        }

        jsonWriter.WriteValue(sb.ToString());

        sb.Length = 0;

        jsonWriter.WriteEndArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteArrayNames(this JsonWriter jsonWriter, string tag, IEnumerable<IdentifiableName>? collection)
    {
        if (collection == null)
        {
            return;
        }

        jsonWriter.WritePropertyName(tag);
        jsonWriter.WriteStartArray();

        var sb = new StringBuilder();

        foreach (var identifiableName in collection)
        {
            sb.Append(identifiableName.Name).Append(',');
        }

        if (sb.Length > 1)
        {
            sb.Length -= 1;
        }

        jsonWriter.WriteValue(sb.ToString());

        sb.Length = 0;

        jsonWriter.WriteEndArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteArray<T>(this JsonWriter jsonWriter, string tag, ICollection<T>? collection) //where T : IJsonSerializable
    {
        if (collection == null)
        {
            return;
        }

        jsonWriter.WritePropertyName(tag);
        jsonWriter.WriteStartArray();

        foreach (var item in collection)
        {
            //  item.WriteJson(jsonWriter);
        }

        jsonWriter.WriteEndArray();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteListAsProperty(this JsonWriter jsonWriter, string tag, ICollection? collection)
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