using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using Padi.RedmineApi.Extensions;

namespace Padi.RedmineApi.Serialization.JsonText.Extensions;

/// <summary>
///
/// </summary>
internal static class Utf8JsonReaderExtensions
{
    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static int ReadAsInt(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetInt32();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static int? ReadAsNullableInt(this Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetInt32();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static decimal ReadAsDecimal(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetDecimal();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static double ReadAsDouble(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetDouble();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static double ReadAsFloat(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetSingle();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static float? ReadAsNullableFloat(this Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetSingle();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static bool ReadAsBool(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetBoolean();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    /// <exception cref="JsonException"></exception>
    public static bool? ReadAsNullableBoolean(this Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetBoolean();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static string? ReadAsString(this Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.Number)
        {
            if (int.TryParse(reader.ValueSpan, NumberStyles.None, NumberFormatInfo.InvariantInfo, out var result))
            {
                return result.ToInvariantString();
            }

            return reader.ValueSpan.ToString();
        }

        return reader.GetString();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static DateTime ReadAsDateTime(this Utf8JsonReader reader)
    {
        reader.Read();
        return reader.GetDateTime();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static DateTime? ReadAsNullableDateTime(this Utf8JsonReader reader)
    {
        reader.Read();
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetDateTime();
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <param name="readInnerArray"></param>
    /// <returns></returns>
    public static List<T>? ReadAsCollection<T>(this ref Utf8JsonReader reader, bool readInnerArray = false)
    {
        List<T>? collection = null;

        var converter = JsonConverterRegistry.GetConverter<T>();

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray)
            {
                break;
            }

            if (readInnerArray)
            {
                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    break;
                }
            }

            if (reader.TokenType == JsonTokenType.StartArray)
            {
                continue;
            }

            var entity = converter.ReadJson(ref reader);

            collection ??= [];
            collection.Add(entity);
        }

        return collection;
    }
}
