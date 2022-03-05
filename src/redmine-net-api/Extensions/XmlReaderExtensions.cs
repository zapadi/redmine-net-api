/*
   Copyright 2011 - 2022 Adrian Popescu

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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// </summary>
    public static class XmlReaderExtensions
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
                return default;
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
                return default;
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
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static List<T> ReadElementContentAsCollection<T>(this XmlReader reader) where T : class
        {
            List<T> result = null;
            var serializer = new XmlSerializer(typeof(T));
            var outerXml = reader.ReadOuterXml();

            if (string.IsNullOrEmpty(outerXml))
            {
                return null;
            }

            using (var stringReader = new StringReader(outerXml))
            {
                using (var xmlTextReader =  XmlTextReaderBuilder.Create(stringReader))
                {
                    xmlTextReader.ReadStartElement();
                    while (!xmlTextReader.EOF)
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.EndElement)
                        {
                            xmlTextReader.ReadEndElement();
                            continue;
                        }

                        T entity;

                        if (xmlTextReader.IsEmptyElement && xmlTextReader.HasAttributes)
                        {
                            entity = serializer.Deserialize(xmlTextReader) as T;
                        }
                        else
                        {
                            if (xmlTextReader.NodeType != XmlNodeType.Element)
                            {
                                xmlTextReader.Read();
                                continue;
                            }

                            var subTree = xmlTextReader.ReadSubtree();
                            entity = serializer.Deserialize(subTree) as T;
                        }

                        if (entity != null)
                        {
                            if (result == null)
                            {
                                result = new List<T>();
                            }

                            result.Add(entity);
                        }

                        if (!xmlTextReader.IsEmptyElement)
                        {
                            xmlTextReader.Read();
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Reads the element content as enumerable.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static IEnumerable<T> ReadElementContentAsEnumerable<T>(this XmlReader reader) where T : class
        {
            var serializer = new XmlSerializer(typeof(T));
            var outerXml = reader.ReadOuterXml();
            
            if (string.IsNullOrEmpty(outerXml))
            {
               yield return null;
            }

            using (var stringReader = new StringReader(outerXml))
            {
                using (var xmlTextReader = XmlTextReaderBuilder.Create(stringReader))
                {
                    xmlTextReader.ReadStartElement();
                    while (!xmlTextReader.EOF)
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.EndElement)
                        {
                            xmlTextReader.ReadEndElement();
                            continue;
                        }

                        T entity;

                        if (xmlTextReader.IsEmptyElement && xmlTextReader.HasAttributes)
                        {
                            entity = serializer.Deserialize(xmlTextReader) as T;
                        }
                        else
                        {
                            var subTree = xmlTextReader.ReadSubtree();
                            entity = serializer.Deserialize(subTree) as T;
                        }
                        if (entity != null)
                        {
                            yield return entity;
                        }

                        if (!xmlTextReader.IsEmptyElement)
                        {
                            xmlTextReader.Read();
                        }
                    }
                }
            }
        }
    }
}