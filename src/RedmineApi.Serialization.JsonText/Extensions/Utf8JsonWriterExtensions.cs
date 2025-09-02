using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.JsonText.Extensions;

/// <summary>
///
/// </summary>
internal static class Utf8JsonWriterExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteIdIfNotNull(this Utf8JsonWriter utf8JsonWriter, string tag, IdentifiableName? value)
    {
        if (value == null)
        {
            return;
        }

        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteNumberValue(value.Id);
    }

    /// <summary>
    /// Writes if not default or null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The property name.</param>
    public static void WriteIfNotDefaultOrNull<T>(this Utf8JsonWriter writer, string elementName, T value) where T : struct
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
            writer.WriteProperty(elementName, value.ToInvariantString());
        }
    }

    /// <summary>
    /// Writes if not default or null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The property name.</param>
    public static void WriteIfNotDefaultOrNull(this Utf8JsonWriter writer, string elementName, string value)
    {
        if (value.IsNullOrWhiteSpace())
        {
            return;
        }

        writer.WriteProperty(elementName, value);
    }

    /// <summary>
    /// Writes the boolean value
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The property name.</param>
    public static void WriteBoolean(this Utf8JsonWriter writer, string elementName, bool value)
    {
        writer.WriteProperty(elementName, value.ToInvariantString());
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="ident"></param>
    /// <param name="emptyValue"></param>
    public static void WriteIdOrEmpty(this Utf8JsonWriter utf8JsonWriter, string tag, IdentifiableName? ident, string? emptyValue = null)
    {
        utf8JsonWriter.WriteProperty(tag, ident != null ? ident.Id.ToInvariantString() : emptyValue);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="val"></param>
    /// <param name="dateFormat"></param>
    public static void WriteDateOrEmpty(this Utf8JsonWriter utf8JsonWriter, string tag, DateTime? val, string dateFormat = "yyyy-MM-dd")
    {
        if (!val.HasValue || val.Value.Equals(default))
        {
            utf8JsonWriter.WriteProperty(tag, string.Empty);
        }
        else
        {
            utf8JsonWriter.WriteProperty(tag, val.Value.ToString(dateFormat, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="val"></param>
    public static void WriteValueOrEmpty<T>(this Utf8JsonWriter utf8JsonWriter, string tag, T? val) where T : struct
    {
        if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default))
        {
            utf8JsonWriter.WriteProperty(tag, string.Empty);
        }
        else
        {
            utf8JsonWriter.WriteProperty(tag, val.Value);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="val"></param>
    public static void WriteValueOrDefault<T>(this Utf8JsonWriter utf8JsonWriter, string tag, T? val) where T : struct
    {
        utf8JsonWriter.WritePropertyName(tag);
        if (val.HasValue)
        {
            utf8JsonWriter.WriteStringValue(val.Value.ToInvariantString());
        }
        else
        {
            var def = default(T);
            utf8JsonWriter.WriteStringValue(def.ToInvariantString());
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteProperty<T>(this Utf8JsonWriter utf8JsonWriter, string tag, T value)
    {
        utf8JsonWriter.WritePropertyName(tag);
        if (value == null)
        {
            utf8JsonWriter.WriteNullValue();
        }
        else
        {
            utf8JsonWriter.WriteStringValue(value.ToString());
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteProperty(this Utf8JsonWriter utf8JsonWriter, string tag, int value)
    {
        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteNumberValue(value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="value"></param>
    public static void WriteProperty(this Utf8JsonWriter utf8JsonWriter, string tag, bool value)
    {
        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteBooleanValue(value);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteRepeatableElement(this Utf8JsonWriter utf8JsonWriter, string tag, IEnumerable<IValue>? collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (var value in collection)
        {
            utf8JsonWriter.WriteProperty(tag, value.Value);
        }
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteArrayIds(this Utf8JsonWriter utf8JsonWriter, string tag, IEnumerable<IdentifiableName>? collection)
    {
        if (collection == null)
        {
            return;
        }

        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteStartArray();

        foreach (var identifiableName in collection)
        {
            utf8JsonWriter.WriteNumberValue(identifiableName.Id);
        }

        utf8JsonWriter.WriteEndArray();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteArrayNames(this Utf8JsonWriter utf8JsonWriter, string tag, IEnumerable<IdentifiableName>? collection)
    {
        if (collection == null)
        {
            return;
        }

        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteStartArray();

        foreach (var identifiableName in collection)
        {
            if (!string.IsNullOrWhiteSpace(identifiableName.Name))
            {
                utf8JsonWriter.WriteStringValue(identifiableName.Name);
            }
        }

        utf8JsonWriter.WriteEndArray();
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    /// <param name="writeRoot"></param>
    public static void WriteArray<T>(this Utf8JsonWriter utf8JsonWriter, string tag, ICollection<T>? collection, bool writeRoot = true)
    {
        if (collection == null)
        {
            return;
        }

        var converter = JsonConverterRegistry.GetConverter<T>();

        utf8JsonWriter.WritePropertyName(tag);
        utf8JsonWriter.WriteStartArray();

        foreach (var item in collection)
        {
            converter.WriteJson(utf8JsonWriter, item,  writeRoot);
        }

        utf8JsonWriter.WriteEndArray();
    }

    public static void WriteArray<T>(this Utf8JsonWriter utf8JsonWriter, string rootName, IEnumerable<T>? collection, Action<T>? action)
    {
        if (collection == null || action == null)
        {
            return;
        }

        utf8JsonWriter.WritePropertyName(rootName);
        utf8JsonWriter.WriteStartArray();

        foreach (var item in collection)
        {
            action.Invoke(item);
        }

        utf8JsonWriter.WriteEndArray();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="utf8JsonWriter"></param>
    /// <param name="tag"></param>
    /// <param name="collection"></param>
    public static void WriteListAsProperty(this Utf8JsonWriter utf8JsonWriter, string tag, ICollection? collection)
    {
        if (collection == null)
        {
            return;
        }

        foreach (var item in collection)
        {
            utf8JsonWriter.WriteProperty(tag, item);
        }
    }
}
