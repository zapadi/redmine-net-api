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
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    public static class XmlReaderExtensions
    {
        /// <summary>
        /// Reads the attribute as int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static int ReadAttributeAsInt(this XmlReader reader, string attributeName)
        {
            var attribute = reader.GetAttribute(attributeName);
            int result;

            if (string.IsNullOrEmpty(attribute) || !int.TryParse(attribute, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) return default(int);

            return result;
        }

        public static int? ReadAttributeAsNullableInt(this XmlReader reader, string attributeName)
        {
            var attribute = reader.GetAttribute(attributeName);
            int result;

            if (string.IsNullOrEmpty(attribute) || !int.TryParse(attribute, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) return default(int?);

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
            bool result;

            if (string.IsNullOrEmpty(attribute) || !bool.TryParse(attribute, out result)) return false;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable date time.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static DateTime? ReadElementContentAsNullableDateTime(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            DateTime result;

            if (string.IsNullOrEmpty(str) || !DateTime.TryParse(str, out result)) return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable float.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static float? ReadElementContentAsNullableFloat(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            float result;

            if (string.IsNullOrEmpty(str) || !float.TryParse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable int.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static int? ReadElementContentAsNullableInt(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            int result;

            if (string.IsNullOrEmpty(str) || !int.TryParse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) return null;

            return result;
        }

        /// <summary>
        /// Reads the element content as nullable decimal.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static decimal? ReadElementContentAsNullableDecimal(this XmlReader reader)
        {
            var str = reader.ReadElementContentAsString();

            decimal result;

            if (string.IsNullOrEmpty(str) || !decimal.TryParse(str, NumberStyles.Any, NumberFormatInfo.InvariantInfo, out result)) return null;

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
            var result = new List<T>();
            var serializer = new XmlSerializer(typeof(T));
            var xml = reader.ReadOuterXml();
            using (var sr = new StringReader(xml))
            {
                using (var xmlTextReader = new XmlTextReader(sr))
                {
                    xmlTextReader.ReadStartElement();
                    while (!xmlTextReader.EOF)
                    {
                        if (xmlTextReader.NodeType == XmlNodeType.EndElement)
                        {
                            xmlTextReader.ReadEndElement();
                            continue;
                        }

                        T obj;

                        if (xmlTextReader.IsEmptyElement && xmlTextReader.HasAttributes)
                        {
                            obj = serializer.Deserialize(xmlTextReader) as T;
                        }
                        else
                        {
                            var subTree = xmlTextReader.ReadSubtree();
                            obj = serializer.Deserialize(subTree) as T;
                        }
                        if (obj != null)
                            result.Add(obj);

                        if (!xmlTextReader.IsEmptyElement)
                            xmlTextReader.Read();
                    }
                }
            }
            return result;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="reader"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static ArrayList ReadElementContentAsCollection(this XmlReader reader, Type type)
        //{
        //    var result = new ArrayList();
        //    var serializer = new XmlSerializer(type);
        //    var xml = reader.ReadOuterXml();
        //    using (var stringReader = new StringReader(xml))
        //    {
        //        using (var xmlTextReader = new XmlTextReader(stringReader))
        //        {
        //            xmlTextReader.ReadStartElement();
        //            while (!xmlTextReader.EOF)
        //            {
        //                if (xmlTextReader.NodeType == XmlNodeType.EndElement)
        //                {
        //                    xmlTextReader.ReadEndElement();
        //                    continue;
        //                }

        //                var subTree = xmlTextReader.ReadSubtree();
        //                var obj = serializer.Deserialize(subTree);
        //                if (obj != null)
        //                    result.Add(obj);
        //                xmlTextReader.Read();
        //            }
        //        }
        //    }
        //    return result;
        //}

        /// <summary>
        /// Writes the id if not null.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="ident">The ident.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteIdIfNotNull(this XmlWriter writer, IdentifiableName ident, string tag)
        {
            if (ident != null) writer.WriteElementString(tag, ident.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="col"></param>
        /// <param name="elementName"></param>
        public static void WriteArray(this XmlWriter writer, IEnumerable col, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (col != null)
            {
                foreach (var item in col)
                {
                    new XmlSerializer(item.GetType()).Serialize(writer, item);
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArrayIds(this XmlWriter writer, IEnumerable col, string elementName, Type type, Func<object, int> f)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (col != null)
            {
                foreach (var item in col)
                {
                    new XmlSerializer(type).Serialize(writer, f.Invoke(item));
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArray(this XmlWriter writer, IEnumerable list, string elementName, Type type, string root, string defaultNamespace = null)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (list != null)
            {
                foreach (var item in list)
                {
                    new XmlSerializer(type, new XmlAttributeOverrides(), new Type[] { }, new XmlRootAttribute(root), defaultNamespace).Serialize(writer, item);
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArrayStringElement(this XmlWriter writer, IEnumerable col, string elementName, Func<object, string> f)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (col != null)
            {
                foreach (var item in col)
                {
                    writer.WriteElementString(elementName, f.Invoke(item));
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteListElements(this XmlWriter xmlWriter, IEnumerable<IValue> list, string elementName)
        {
            if (list == null)
                return;

            foreach (var item in list)
            {
                xmlWriter.WriteElementString(elementName, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="ident"></param>
        /// <param name="tag"></param>
        public static void WriteIdOrEmpty(this XmlWriter writer, IdentifiableName ident, string tag)
        {
            writer.WriteElementString(tag, ident != null ? ident.Id.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer"></param>
        /// <param name="val"></param>
        /// <param name="tag"></param>
        public static void WriteIfNotDefaultOrNull<T>(this XmlWriter writer, T? val, string tag) where T : struct
        {
            if (!val.HasValue) return;
            if (!EqualityComparer<T>.Default.Equals(val.Value, default(T)))
                writer.WriteElementString(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value));
        }

        /// <summary>
        /// Writes string empty if T has default value or null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteValueOrEmpty<T>(this XmlWriter writer, T? val, string tag) where T : struct
        {
            if (!val.HasValue || EqualityComparer<T>.Default.Equals(val.Value, default(T)))
                writer.WriteElementString(tag, string.Empty);
            else
                writer.WriteElementString(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="val"></param>
        /// <param name="tag"></param>
        public static void WriteDateOrEmpty(this XmlWriter writer, DateTime? val, string tag)
        {
            if (!val.HasValue || val.Value.Equals(default(DateTime)))
                writer.WriteElementString(tag, string.Empty);
            else
                writer.WriteElementString(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }
    }
}