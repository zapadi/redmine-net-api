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
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Internals;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    public static class XmlWriterExtensions
    {
        /// <summary>
        /// Writes the id if not null.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="identifiableName">The ident.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteIdIfNotNull(this XmlWriter writer, IdentifiableName identifiableName, string tag)
        {
            if (identifiableName != null) writer.WriteElementString(tag, identifiableName.Id.ToString(CultureInfo.InvariantCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="enumerable"></param>
        /// <param name="elementName"></param>
        public static void WriteArray(this XmlWriter writer, IEnumerable enumerable, string elementName)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    new XmlSerializer(item.GetType()).Serialize(writer, item);
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArray(this XmlWriter writer, IEnumerable enumerable, string elementName, Type type, string root, string defaultNamespace = null)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    new XmlSerializer(type, new XmlAttributeOverrides(), new Type[] { }, new XmlRootAttribute(root), defaultNamespace).Serialize(writer, item);
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArrayIds(this XmlWriter writer, IEnumerable enumerable, string elementName, Type type, Func<object, int> f)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    new XmlSerializer(type).Serialize(writer, f.Invoke(item));
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteArrayStringElement(this XmlWriter writer, IEnumerable enumerable, string elementName, Func<object, string> f)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");
            if (enumerable != null)
            {
                foreach (var item in enumerable)
                {
                    writer.WriteElementString(elementName, f.Invoke(item));
                }
            }
            writer.WriteEndElement();
        }

        public static void WriteListElements(this XmlWriter xmlWriter, IEnumerable<IValue> enumerable, string elementName)
        {
            if (enumerable == null) return;

            foreach (var item in enumerable)
            {
                xmlWriter.WriteElementString(elementName, item.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="identifiableName"></param>
        /// <param name="tag"></param>
        public static void WriteIdOrEmpty(this XmlWriter writer, IdentifiableName identifiableName, string tag)
        {
            writer.WriteElementString(tag, identifiableName != null ? identifiableName.Id.ToString(CultureInfo.InvariantCulture) : string.Empty);
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
        /// <param name="dateTime"></param>
        /// <param name="xmlTag"></param>
        public static void WriteDateOrEmpty(this XmlWriter writer, DateTime? dateTime, string xmlTag)
        {
            if (!dateTime.HasValue || dateTime.Value.Equals(default(DateTime)))
                writer.WriteElementString(xmlTag, string.Empty);
            else
                writer.WriteElementString(xmlTag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", dateTime.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }
    }
}