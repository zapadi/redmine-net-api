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
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Redmine.Net.Api.Extensions;
using Redmine.Net.Api.Types;
using Redmine.Net.Api.Exceptions;

namespace Redmine.Net.Api.Internals
{
    internal static partial class RedmineSerializer
    {
        /// <summary>
        /// Serializes the specified System.Object and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T">The type of objects to serialize.</typeparam>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>
        /// The System.String that contains the XML document.
        /// </returns>
        /// <exception cref="InvalidOperationException"></exception>
        // ReSharper disable once InconsistentNaming
        private static string ToXML<T>(T obj) where T : class
        {
            var xws = new XmlWriterSettings {OmitXmlDeclaration = true};
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter, xws))
                {
                    var sr = new XmlSerializer(typeof (T));
                    sr.Serialize(xmlWriter, obj);
                    return stringWriter.ToString();
                }
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T">The type of objects to deserialize.</typeparam>
        /// <param name="xml">The System.String that contains the XML document to deserialize.</param>
        /// <returns>
        /// The T object being deserialized.
        /// </returns>
        /// <exception cref="System.InvalidOperationException">An error occurred during deserialization. The original exception is available
        /// using the System.Exception.InnerException property.</exception>
        // ReSharper disable once InconsistentNaming
        private static T FromXML<T>(string xml) where T : class
        {
            using (var text = new StringReader(xml))
            {
                var sr = new XmlSerializer(typeof (T));
                return sr.Deserialize(text) as T;
            }
        }

        /// <summary>
        /// Serializes the specified type T and writes the XML document to a string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException">Serialization error</exception>
        public static string Serialize<T>(T obj, MimeFormat mimeFormat) where T : class, new()
        {
            try
            {
                if (mimeFormat == MimeFormat.Json)
                {
                    return JsonSerializer(obj);
                }
                return ToXML(obj);
            }
            catch (Exception ex)
            {
                throw new RedmineException("Serialization error", ex);
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specific System.String.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException">
        /// Could not deserialize null!
        /// or
        /// Deserialization error
        /// </exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static T Deserialize<T>(string response, MimeFormat mimeFormat) where T : class, new()
        {
            if (string.IsNullOrEmpty(response)) throw new RedmineException("Could not deserialize null!");
            try
            {
                if (mimeFormat == MimeFormat.Json)
                {
                    var type = typeof (T);
                    var jsonRoot = (string) null;
                    if (type == typeof (IssueCategory)) jsonRoot = RedmineKeys.ISSUE_CATEGORY;
                    if (type == typeof (IssueRelation)) jsonRoot = RedmineKeys.RELATION;
                    if (type == typeof (TimeEntry)) jsonRoot = RedmineKeys.TIME_ENTRY;
                    if (type == typeof (ProjectMembership)) jsonRoot = RedmineKeys.MEMBERSHIP;
                    if (type == typeof (WikiPage)) jsonRoot = RedmineKeys.WIKI_PAGE;
                    return JsonDeserialize<T>(response, jsonRoot);
                }

                return FromXML<T>(response);
            }
            catch (Exception ex)
            {
                throw new RedmineException("Deserialization error",ex);
            }
        }

        /// <summary>
        /// Deserializes the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <param name="mimeFormat">The MIME format.</param>
        /// <returns></returns>
        /// <exception cref="RedmineException">
        /// Could not deserialize null!
        /// or
        /// Deserialization error
        /// </exception>
        public static PaginatedObjects<T> DeserializeList<T>(string response, MimeFormat mimeFormat)
            where T : class, new()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(response)) throw new RedmineException("Could not deserialize null!");

                if (mimeFormat == MimeFormat.Json)
                {
                    return JSonDeserializeList<T>(response);
                }

                return XmlDeserializeList<T>(response);
            }

            catch (Exception ex)
            {
                throw new RedmineException("Deserialization error", ex);
            }
        }

        /// <summary>
        /// js the son deserialize list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private static PaginatedObjects<T> JSonDeserializeList<T>(string response) where T : class, new()
        {

            int totalItems, offset;
            var type = typeof(T);
            var jsonRoot = (string)null;
            if (type == typeof(Error)) jsonRoot = RedmineKeys.ERRORS;
            if (type == typeof(WikiPage)) jsonRoot = RedmineKeys.WIKI_PAGES;
            if (type == typeof(IssuePriority)) jsonRoot = RedmineKeys.ISSUE_PRIORITIES;
            if (type == typeof(TimeEntryActivity)) jsonRoot = RedmineKeys.TIME_ENTRY_ACTIVITIES;

            if (string.IsNullOrEmpty(jsonRoot))
                jsonRoot = RedmineManager.Sufixes[type];

            var result = JsonDeserializeToList<T>(response, jsonRoot, out totalItems, out offset);

            return new PaginatedObjects<T>()
            {
                TotalCount = totalItems,
                Offset = offset,
                Objects = result.ToList()
            };
        }

        /// <summary>
        /// XMLs the deserialize list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response">The response.</param>
        /// <returns></returns>
        private static PaginatedObjects<T> XmlDeserializeList<T>(string response) where T : class, new()
        {
            using (var stringReader = new StringReader(response))
            {
                using (var xmlReader = new XmlTextReader(stringReader))
                {
                    xmlReader.WhitespaceHandling = WhitespaceHandling.None;
                    xmlReader.Read();
                    xmlReader.Read();

                    var totalItems = xmlReader.ReadAttributeAsInt(RedmineKeys.TOTAL_COUNT);
                    var offset = xmlReader.ReadAttributeAsInt(RedmineKeys.OFFSET);
                    var result = xmlReader.ReadElementContentAsCollection<T>();
                    return new PaginatedObjects<T>()
                    {
                        TotalCount = totalItems,
                        Offset = offset,
                        Objects = result
                    };
                }
            }
        }
    }
}