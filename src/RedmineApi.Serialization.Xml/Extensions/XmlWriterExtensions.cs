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
using System.Xml;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.Xml.Extensions;

/// <summary>
/// 
/// </summary>
public static class XmlWriterExtensions
{
    /// <summary>
    /// Writes the id if not null.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="elementName"></param>
    /// <param name="identifiableName"></param>
    public static void WriteIdIfNotNull(this XmlWriter writer, string elementName, IdentifiableName? identifiableName)
    {
        if (identifiableName != null)
        {
            writer.WriteElementString(elementName, StringExtensions.ToInvariantString<int>(identifiableName.Id));
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="elementName"></param>
    /// <param name="ident"></param>
    public static void WriteIdOrEmpty(this XmlWriter writer, string elementName, IdentifiableName? ident)
    {
        writer.WriteElementString(elementName, ident != null ? StringExtensions.ToInvariantString<int>(ident.Id) : string.Empty);
    }

    /// <summary>
    /// Writes if not default or null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The tag.</param>
    public static void WriteIfNotDefaultOrNull<T>(this XmlWriter writer, string elementName, T? value)
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
        {
            return;
        }
            
        if (value is bool boolValue)
        {
            writer.WriteElementString(elementName, boolValue.ToInvariantString());
        }
        else
        {
            writer.WriteElementString(elementName, value.ToString());
        }
    }

    /// <summary>
    /// Writes the boolean value
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="value">The value.</param>
    /// <param name="elementName">The tag.</param>
    public static void WriteBoolean(this XmlWriter writer, string elementName, bool value)
    {
        writer.WriteElementString(elementName, value.ToInvariantString());
    }

    /// <summary>
    /// Writes string empty if T has default value or null.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="writer">The writer.</param>
    /// <param name="val">The value.</param>
    /// <param name="elementName">The tag.</param>
    public static void WriteValueOrEmpty<T>(this XmlWriter writer, string elementName, T? val) where T : struct
    {
        if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default))
        {
            writer.WriteElementString(elementName, string.Empty);
        }
        else
        {
            writer.WriteElementString(elementName, val.Value.ToInvariantString());
        }
    }

    /// <summary>
    /// Writes the date or empty.
    /// </summary>
    /// <param name="writer">The writer.</param>
    /// <param name="elementName">The tag.</param>
    /// <param name="val">The value.</param>
    /// <param name="dateTimeFormat"></param>
    public static void WriteDateOrEmpty(this XmlWriter writer, string elementName, DateTime? val, string dateTimeFormat = "yyyy-MM-dd")
    {
        if (!val.HasValue || val.Value.Equals(default))
        {
            writer.WriteElementString(elementName, string.Empty);
        }
        else
        {
            writer.WriteElementString(elementName, val.Value.ToString(dateTimeFormat, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Writes the list elements.
    /// </summary>
    /// <param name="xmlWriter">The XML writer.</param>
    /// <param name="collection">The collection.</param>
    /// <param name="elementName">Name of the element.</param>
    public static void WriteListElements<T>(this XmlWriter xmlWriter, string elementName, List<T> collection) where T : IValue
    {
        WriteRepeatableElement(xmlWriter, elementName, collection);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlWriter"></param>
    /// <param name="elementName"></param>
    /// <param name="collection"></param>
    public static void WriteRepeatableElement<T>(this XmlWriter xmlWriter, string elementName, List<T>? collection) where T : IValue
    {
        if (collection == null)
        {
            return;
        }

        foreach (var item in collection)
        {
            xmlWriter.WriteElementString(elementName, item.Value);
        }
    }

    public static void WriteArrayIds<T>(this XmlWriter writer, string rootName, string elementName, List<T> collection, Func<T, string> f)
    {
        WriteArray(writer, rootName, elementName, collection, f);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="elementName"></param>
    /// <param name="collection"></param>
    /// <param name="f"></param>
    public static void WriteArrayStringElement(this XmlWriter writer, string elementName, IEnumerable collection, Func<object, string> f)
    {
        WriteArray(writer, elementName, elementName, collection, f);
    }

    public static void WriteArray(this XmlWriter writer, string rootName, string elementName, IEnumerable collection, Func<object, string> valueFunc)
    {
        WriteArray(writer, rootName, elementName, (IEnumerable<object>)collection, valueFunc);
    }

    public static void WriteArray<T>(this XmlWriter writer, string? rootName, IEnumerable<T>? collection)
    {
        if (collection == null)
        {
            return;
        }

        if (rootName != null)
        {
            writer.WriteStartElement(rootName);
            writer.WriteAttributeString(Constants.TYPE, Constants.ARRAY);
        }

        var converter = XmlConverterRegistry.GetConverter<T>();

        foreach (var item in collection)
        {
            converter.WriteXml(writer, item);
        }

        if (rootName != null)
        {
            writer.WriteEndElement();
        }
    }

    public static void WriteArray<T>(this XmlWriter writer, string rootName, string elementName, IEnumerable<T>? collection, Func<T, string> valueFunc)
    {
        if (collection == null)
        {
            return;
        }

        writer.WriteStartElement(rootName);
        writer.WriteAttributeString(Constants.TYPE, Constants.ARRAY);

        foreach (var item in collection)
        {
            writer.WriteElementString(elementName, valueFunc.Invoke(item));
        }

        writer.WriteEndElement();
    }

    public static void WriteArray<T>(this XmlWriter writer, string rootName, IEnumerable<T>? collection, Action<T>? action)
    {
        if (collection == null || action == null)
        {
            return;
        }

        writer.WriteStartElement(rootName);
        writer.WriteAttributeString(Constants.TYPE, Constants.ARRAY);

        foreach (var item in collection)
        {
            action.Invoke(item);
        }

        writer.WriteEndElement();
    }
}