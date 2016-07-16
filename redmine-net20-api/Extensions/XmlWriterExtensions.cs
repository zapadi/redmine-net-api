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
    /// <summary>
    /// 
    /// </summary>
    public static partial class XmlExtensions
    {
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
        /// Writes the array.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void WriteArray(this XmlWriter writer, IEnumerable collection, string elementName)
        {
            if (collection == null) return;
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the array ids.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="type">The type.</param>
        /// <param name="f">The f.</param>
        public static void WriteArrayIds(this XmlWriter writer, IEnumerable collection, string elementName, Type type, Func<object, int> f)
        {
            if (collection == null) return;
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                new XmlSerializer(type).Serialize(writer, f.Invoke(item));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="type">The type.</param>
        /// <param name="root">The root.</param>
        /// <param name="defaultNamespace">The default namespace.</param>
        public static void WriteArray(this XmlWriter writer, IEnumerable collection, string elementName, Type type, string root, string defaultNamespace = null)
        {
            if (collection == null) return;
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                new XmlSerializer(type, new XmlAttributeOverrides(), new Type[] { }, new XmlRootAttribute(root),
                    defaultNamespace).Serialize(writer, item);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the array string element.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="f">The func to invoke.</param>
        public static void WriteArrayStringElement(this XmlWriter writer, IEnumerable collection, string elementName, Func<object, string> f)
        {
            if (collection == null) return;
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                writer.WriteElementString(elementName, f.Invoke(item));
            }
            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the list elements.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void WriteListElements(this XmlWriter xmlWriter, IEnumerable<IValue> collection, string elementName)
        {
            if (collection == null) return;

            foreach (var item in collection)
            {
                xmlWriter.WriteElementString(elementName, item.Value);
            }
        }

        /// <summary>
        /// Writes the identifier or empty.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="ident">The ident.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteIdOrEmpty(this XmlWriter writer, IdentifiableName ident, string tag)
        {
            writer.WriteElementString(tag, ident != null ? ident.Id.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }

        /// <summary>
        /// Writes if not default or null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
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
        /// Writes the date or empty.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="val">The value.</param>
        /// <param name="tag">The tag.</param>
        public static void WriteDateOrEmpty(this XmlWriter writer, DateTime? val, string tag)
        {
            if (!val.HasValue || val.Value.Equals(default(DateTime)))
                writer.WriteElementString(tag, string.Empty);
            else
                writer.WriteElementString(tag, string.Format(NumberFormatInfo.InvariantInfo, "{0}", val.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)));
        }
    }
}