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
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Redmine.Net.Api.Exceptions;
using Redmine.Net.Api.Extensions;

namespace Redmine.Net.Api.Serialization
{
    /// <summary>
    /// 
    /// </summary>
    internal static class SerializationHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="redmineSerializer"></param>
        /// <returns></returns>
        public static string SerializeUserId(int userId, IRedmineSerializer redmineSerializer)
        {
            return redmineSerializer is XmlRedmineSerializer
                ? $"<user_id>{userId.ToInvariantString()}</user_id>"
                : $"{{\"user_id\":\"{userId.ToInvariantString()}\"}}";
        }

        public static string SerializeUserIds(IEnumerable<int> userIds, string root, IRedmineSerializer redmineSerializer)
        {
            var ids = EnsureUserIds(userIds);

            if (redmineSerializer is XmlRedmineSerializer)
            {
                return WriteXml(writer =>
                {
                    if (root != null)
                    {
                        writer.WriteStartElement(root);
                    }

                    writer.WriteStartElement(RedmineKeys.USER_IDS);
                    writer.WriteAttributeString("type", "array");
                    foreach (var id in ids)
                    {
                        writer.WriteElementString(RedmineKeys.USER_ID, id.ToInvariantString());
                    }
                    writer.WriteEndElement();

                    if (root != null)
                    {
                        writer.WriteEndElement();
                    }
                });
            }

            return WriteJson(writer =>
            {
                using (new JsonObject(writer, root))
                {
                    writer.WritePropertyName(RedmineKeys.USER_IDS);
                    writer.WriteStartArray();
                    foreach (var id in ids)
                    {
                        writer.WriteValue(id);
                    }
                    writer.WriteEndArray();
                }
            });
        }

        public static List<int> EnsureUserIds(IEnumerable<int> userIds)
        {
            var ids = userIds == null ? null : new List<int>(userIds);

            if (ids == null || ids.Count == 0)
            {
                throw new RedmineException("At least one user id is required.");
            }

            return ids;
        }

        public static string SerializeJournal(string notes, bool? privateNotes, IRedmineSerializer redmineSerializer)
        {
            if (redmineSerializer is XmlRedmineSerializer)
            {
                return WriteXml(writer =>
                {
                    writer.WriteStartElement(RedmineKeys.JOURNAL);
                    if (notes != null)
                    {
                        writer.WriteElementString(RedmineKeys.NOTES, notes);
                    }

                    if (privateNotes.HasValue)
                    {
                        writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, privateNotes.Value);
                    }
                    writer.WriteEndElement();
                });
            }

            return WriteJson(writer =>
            {
                using (new JsonObject(writer, RedmineKeys.JOURNAL))
                {
                    if (notes != null)
                    {
                        writer.WriteProperty(RedmineKeys.NOTES, notes);
                    }

                    if (privateNotes.HasValue)
                    {
                        writer.WriteBoolean(RedmineKeys.PRIVATE_NOTES, privateNotes.Value);
                    }
                }
            });
        }

        private static string WriteXml(Action<XmlWriter> write)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (var writer = XmlWriter.Create(stringWriter, new XmlWriterSettings { OmitXmlDeclaration = true }))
                {
                    write(writer);
                }
            }

            return sb.ToString();
        }

        private static string WriteJson(Action<JsonWriter> write)
        {
            var sb = new StringBuilder();
            using (var stringWriter = new StringWriter(sb, CultureInfo.InvariantCulture))
            {
                using (var writer = new JsonTextWriter(stringWriter))
                {
                    write(writer);
                }
            }

            return sb.ToString();
        }
    }
}