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
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Padi.RedmineApi.Extensions;
using Padi.RedmineApi.Types;

namespace Padi.RedmineApi.Serialization.Xml.Extensions;

/// <summary>
/// 
/// </summary>
internal static class XmlReaderExtensions
{
    /// <summary>
    /// Date time format for journals, attachments etc.
    /// </summary>
    private const string INCLUDE_DATE_TIME_FORMAT = "yyyy'-'MM'-'dd HH':'mm':'ss UTC";

    /// <summary>
    /// Reads the attribute as int.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    public static int ReadAttributeAsInt(this XmlReader reader, string attributeName)
    {
        var attribute = reader.GetAttribute(attributeName);

        if (attribute.IsNullOrWhiteSpace() || !int.TryParse(attribute, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
        {
            return 0;
        }

        return result;
    }

    /// <summary>
    /// Reads the attribute as nullable int.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    public static int? ReadAttributeAsNullableInt(this XmlReader reader, string attributeName)
    {
        var attribute = reader.GetAttribute(attributeName);

        if (attribute.IsNullOrWhiteSpace() || !int.TryParse(attribute, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the attribute as boolean.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <param name="attributeName">Name of the attribute.</param>
    /// <returns></returns>
    public static bool ReadAttributeAsBoolean(this XmlReader reader, string attributeName)
    {
        var attribute = reader.GetAttribute(attributeName);

        if (attribute.IsNullOrWhiteSpace() || !bool.TryParse(attribute, out var result))
        {
            return false;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as nullable boolean.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static bool? ReadElementContentAsNullableBoolean(this XmlReader reader)
    {
        var content = reader.ReadElementContentAsString();

        if (content.IsNullOrWhiteSpace() || !bool.TryParse(content, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as nullable date time.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static DateTime? ReadElementContentAsNullableDateTime(this XmlReader reader)
    {
        var content = reader.ReadElementContentAsString();

        if (!content.IsNullOrWhiteSpace() && DateTime.TryParse(content, out var result))
        {
            return result;
        }

        if (!DateTime.TryParseExact(content, INCLUDE_DATE_TIME_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as nullable float.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static float? ReadElementContentAsNullableFloat(this XmlReader reader)
    {
        var content = reader.ReadElementContentAsString();

        if (content.IsNullOrWhiteSpace() || !float.TryParse(content, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as nullable int.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static int? ReadElementContentAsNullableInt(this XmlReader reader)
    {
        var content = reader.ReadElementContentAsString();

        if (content.IsNullOrWhiteSpace() || !int.TryParse(content, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as nullable decimal.
    /// </summary>
    /// <param name="reader">The reader.</param>
    /// <returns></returns>
    public static decimal? ReadElementContentAsNullableDecimal(this XmlReader reader)
    {
        var content = reader.ReadElementContentAsString();

        if (content.IsNullOrWhiteSpace() || !decimal.TryParse(content, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
        {
            return null;
        }

        return result;
    }

    /// <summary>
    /// Reads the element content as collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="xmlReader"></param>
    /// <returns></returns>
    public static List<T>? ReadElementContentAsCollection<T>(this XmlReader xmlReader) where T : new()
    {
        List<T>? result = null;

        if (xmlReader.IsEmptyElement)
        {
            xmlReader.Read();
            return null;
        }

        var converter = XmlConverterRegistry.GetConverter<T>();

        xmlReader.ReadStartElement();

        while (xmlReader.NodeType != XmlNodeType.EndElement && !xmlReader.EOF)
        {
            if (xmlReader.NodeType == XmlNodeType.Element)
            {
                using (var subTree = xmlReader.ReadSubtree())
                {
                    subTree.Read();
                    var entity = converter.ReadXml(subTree);
                    if (entity != null)
                    {
                        result ??= [];
                        result.Add(entity);
                    }
                }

                xmlReader.Skip();
            }
            else
            {
                xmlReader.Read();
            }
        }

        if (xmlReader.NodeType == XmlNodeType.EndElement)
        {
            xmlReader.ReadEndElement();
        }

        return result;
    }

    internal static IdentifiableName? ReadAsIdentifiableName(this XmlReader reader)
    {
        var converter = XmlConverterRegistry.GetConverter<IdentifiableName>();
        return converter.ReadXml(reader);
    }

    internal static IdentifiableName? ReadAttributesAsIdentifiableName(this XmlReader reader)
    {
        if (!reader.HasAttributes)
        {
            return null;
        }
            
        var id = reader.ReadAttributeAsInt(RedmineKeys.ID);
        var name = reader.GetAttribute(RedmineKeys.NAME);

        return new IdentifiableName(id, name);
    }
}