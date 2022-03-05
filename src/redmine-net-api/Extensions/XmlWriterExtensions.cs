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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Types;

namespace Redmine.Net.Api.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static partial class XmlExtensions
    {

#if !(NET20 || NET40 || NET45 || NET451 || NET452)
        private static readonly Type[] EmptyTypeArray = Array.Empty<Type>();
#else
        private static readonly Type[] EmptyTypeArray = new Type[0];
#endif
        private static readonly XmlAttributeOverrides XmlAttributeOverrides = new XmlAttributeOverrides();

        /// <summary>
        /// Writes the id if not null.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="elementName"></param>
        /// <param name="identifiableName"></param>
        public static void WriteIdIfNotNull(this XmlWriter writer, string elementName, IdentifiableName identifiableName)
        {
            if (identifiableName != null)
            {
                writer.WriteElementString(elementName, identifiableName.Id.ToString(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Writes the array.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void WriteArray(this XmlWriter writer, string elementName, IEnumerable collection)
        {
            if (collection == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                new XmlSerializer(item.GetType()).Serialize(writer, item);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        ///     Writes the array.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void WriteArray<T>(this XmlWriter writer, string elementName, IEnumerable<T> collection)
        {
            if (collection == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            var serializer = new XmlSerializer(typeof(T));

            foreach (var item in collection)
            {
                serializer.Serialize(writer, item);
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
        public static void WriteArrayIds(this XmlWriter writer, string elementName, IEnumerable collection, Type type, Func<object, int> f)
        {
            if (collection == null || f == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            var serializer = new XmlSerializer(type);

            foreach (var item in collection)
            {
                serializer.Serialize(writer, f.Invoke(item));
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
        public static void WriteArray(this XmlWriter writer, string elementName, IEnumerable collection, Type type, string root, string defaultNamespace = null)
        {
            if (collection == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            var rootAttribute = new XmlRootAttribute(root);

            var serializer = new XmlSerializer(type, XmlAttributeOverrides, EmptyTypeArray, rootAttribute,
                defaultNamespace);

            foreach (var item in collection)
            {
                serializer.Serialize(writer, item);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Writes the list elements.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="collection">The collection.</param>
        /// <param name="elementName">Name of the element.</param>
        public static void WriteListElements(this XmlWriter xmlWriter, string elementName, IEnumerable<IValue> collection)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlWriter"></param>
        /// <param name="elementName"></param>
        /// <param name="collection"></param>
        public static void WriteRepeatableElement(this XmlWriter xmlWriter, string elementName, IEnumerable<IValue> collection)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="collection"></param>
        /// <param name="f"></param>
        public static void WriteArrayStringElement(this XmlWriter writer, string elementName, IEnumerable collection, Func<object, string> f)
        {
            if (collection == null)
            {
                return;
            }

            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("type", "array");

            foreach (var item in collection)
            {
                writer.WriteElementString(elementName, f.Invoke(item));
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="elementName"></param>
        /// <param name="ident"></param>
        public static void WriteIdOrEmpty(this XmlWriter writer, string elementName, IdentifiableName ident)
        {
            writer.WriteElementString(elementName, ident != null ? ident.Id.ToString(CultureInfo.InvariantCulture) : string.Empty);
        }

        /// <summary>
        /// Writes if not default or null.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="writer">The writer.</param>
        /// <param name="value">The value.</param>
        /// <param name="elementName">The tag.</param>
        public static void WriteIfNotDefaultOrNull<T>(this XmlWriter writer, string elementName, T value)
        {
            if (EqualityComparer<T>.Default.Equals(value, default))
            {
                return;
            }

            if (value is bool)
            {
                writer.WriteElementString(elementName, value.ToString().ToLowerInv());
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
            writer.WriteElementString(elementName, value.ToString().ToLowerInv());
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
                writer.WriteElementString(elementName, val.Value.ToString().ToLowerInv());
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
    }
}